using SqlParser;
using SqlParser.Ast;
using SqlParser.Tokens;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using static SqlParser.Ast.ColumnOption;

namespace TowerSoft.DevToolkit.Components.PocoGenerator {
    public partial class PocoGenerator {

        string inputText;
        InputType inputType;
        string outputText;

        public bool errorOccurred;

        private void Copy() {
            System.Windows.Clipboard.SetText(outputText);
        }

        internal async Task Generate() {
            errorOccurred = false;
            outputText = string.Empty;

            if (string.IsNullOrWhiteSpace(inputText)) {
                errorOccurred = true;
                outputText = "No input text found.";
                return;
            }

            string converted = inputText.Replace("USING BTREE", "", StringComparison.OrdinalIgnoreCase);

            Regex collateRegex = new(@"COLLATE='(.*?)'");
            converted = collateRegex.Replace(converted, "");

            Regex engineRegex = new(@"ENGINE=.*?\W");
            converted = engineRegex.Replace(converted, "");

            Regex autoIncrementRegex = new(@"AUTO_INCREMENT=\d*?\W");
            converted = autoIncrementRegex.Replace(converted, "");

            Regex uniqueNameRegex = new(@"(?<=UNIQUE INDEX )`.*?`");
            converted = uniqueNameRegex.Replace(converted, "");

            try {
                Sequence<Statement> sql = new SqlQueryParser().Parse(converted);
                Statement.CreateTable statement = sql.First() as Statement.CreateTable;
                Console.WriteLine();
                CreateTable q = statement.Element;

                StringBuilder sb = new();
                sb.AppendLine("public class " + q.Name.Values.First().Value + " {");

                int count = 1;
                foreach (ColumnDef c in q.Columns) {
                    List<string> attributes = GetAttributes(c, q);
                    if (attributes.Any())
                        sb.AppendLine("    [" + string.Join(", ", attributes) + "]");

                    string type = GetDataType(c);

                    sb.AppendLine("    public " + type + " " + c.Name.Value + " { get; set; }");

                    if (count < q.Columns.Count)
                        sb.AppendLine();
                    count++;
                }
                sb.Append("}");

                outputText = sb.ToString();
            } catch (Exception ex) {
                errorOccurred = true;
                outputText = ex.Message;

                if (ex.InnerException != null) {
                    outputText += Environment.NewLine + "Inner Exception: " + ex.InnerException.Message;
                }
            }

        }

        private List<string> GetAttributes(ColumnDef c, CreateTable table) {
            List<string> attributes = [];

            bool autonumberFound = false;
            bool singlePrimaryKeyFound = false;

            if (c.Options != null) {
                foreach (ColumnOptionDef option in c.Options) {
                    if (option.Option is not DialectSpecific dialectSpec) continue;

                    foreach (Token token in dialectSpec.Tokens) {
                        if (token is not Word word) continue;

                        if (word.Value == "AUTO_INCREMENT") {
                            attributes.Add("Autonumber");
                            autonumberFound = true;
                        }
                    }
                }
            }

            if (!autonumberFound && c.Options != null) {
                foreach (ColumnOptionDef option in c.Options) {
                    PropertyInfo prop = option.Option.GetType().GetProperty("IsPrimary");
                    if (prop != null) {
                        bool isPrimary = (bool)prop.GetValue(option.Option);

                        if (isPrimary) {
                            attributes.Add("KeyMap");
                            singlePrimaryKeyFound = true;
                        }
                    }
                }
            }

            if (!autonumberFound && !singlePrimaryKeyFound && table.Constraints != null) {
                foreach (TableConstraint constraint in table.Constraints) {
                    if (constraint is not TableConstraint.Unique u) continue;
                    if (!u.IsPrimaryKey) continue;

                    foreach (Ident column in u.Columns) {
                        if (c.Name.Value == column.Value) {
                            attributes.Add("KeyMap");
                        }
                    }
                }
            }

            if (c.DataType is DataType.Varchar || c.DataType is DataType.Nvarchar) {
                if (c.Options != null && c.Options.Any(x => x.Option is NotNull)) {
                    attributes.Add("Required");
                }

                object length;
                if (c.DataType is DataType.Varchar) {
                    DataType.Varchar type = c.DataType as DataType.Varchar;
                    length = type.CharacterLength.GetType().GetProperty("Length").GetValue(type.CharacterLength);
                } else {
                    DataType.Nvarchar type = c.DataType as DataType.Nvarchar;
                    length = type.CharacterLength.GetType().GetProperty("Length").GetValue(type.CharacterLength);
                }

                attributes.Add("MaxLength(" + length + ")");
            }

            return attributes;
        }

        private string GetDataType(ColumnDef c) {
            string type = "Unhandled_Data_Type";
            switch (c.DataType) {
                case DataType.BigInt:
                case DataType.Int64:
                case DataType.Numeric:
                    type = "long";
                    break;
                case DataType.UInt64:
                case DataType.UnsignedBigInt:
                    type = "ulong";
                    break;
                case DataType.Int:
                case DataType.Int32:
                case DataType.Integer:
                case DataType.MediumInt:
                    type = "int";
                    break;
                case DataType.UInt32:
                case DataType.UnsignedInt:
                case DataType.UnsignedMediumInt:
                    type = "uint";
                    break;
                case DataType.Int16:
                case DataType.SmallInt:
                    type = "short";
                    break;
                case DataType.UInt8:
                case DataType.UInt16:
                case DataType.UnsignedSmallInt:
                case DataType.UnsignedInt8:
                    type = "ushort";
                    break;
                case DataType.TinyInt:
                    type = "bool";
                    break;
                case DataType.Blob:
                case DataType.Varbinary:
                    type = "byte[]";
                    break;
                case DataType.Date:
                case DataType.Datetime:
                case DataType.Timestamp:
                    type = "DateTime";
                    break;
                case DataType.Decimal:
                    type = "decimal";
                    break;
                case DataType.Double:
                case DataType.DoublePrecision:
                    type = "double";
                    break;
                case DataType.Float:
                    type = "float";
                    break;
                case DataType.Nvarchar:
                case DataType.Varchar:
                case DataType.StringType:
                case DataType.Text:
                case DataType.Json:
                    type = "string";
                    break;
                case DataType.Custom:
                    var custom = c.DataType as DataType.Custom;
                    if (custom.Name.Values.Contains("MEDIUMTEXT")) {
                        type = "string";
                    }

                    if (custom.Name.Values.Contains("MEDIUMBLOB")) {
                        type = "byte[]";
                    }

                    Console.WriteLine();
                    break;
                default:
                    Console.WriteLine();
                    break;
            }

            if (c.Options != null && c.Options.Any(x => x.Option is Null) && type != "string") {
                type += "?";
            }

            return type;
        }
    }


    public enum InputType {
        SQL,
        [System.ComponentModel.DataAnnotations.Display(Name = "JSON/XML")]
        JsonXml
    }
}

/*

CREATE TABLE `cat` (
`ID` BIGINT(19) NOT NULL AUTO_INCREMENT,
`FosterGroupID` BIGINT(19) NOT NULL,
`ImageID` BIGINT(19) NULL DEFAULT NULL,
`Name` VARCHAR(100) NOT NULL COLLATE 'utf8mb4_0900_ai_ci',
`Description` VARCHAR(255) NULL DEFAULT NULL COLLATE 'utf8mb4_0900_ai_ci',
`Gender` INT(10) NULL DEFAULT NULL,
`HumaneSocietyNumber` VARCHAR(45) NULL DEFAULT NULL COLLATE 'utf8mb4_0900_ai_ci',
`IsActive` TINYINT(3) NOT NULL,
PRIMARY KEY (`ID`) USING BTREE,
INDEX `fk_cat_fostergroupid` (`FosterGroupID`) USING BTREE,
INDEX `fk_cat_imageid` (`ImageID`) USING BTREE,
CONSTRAINT `fk_cat_fostergroupid` FOREIGN KEY (`FosterGroupID`) REFERENCES `fostergroup` (`ID`) ON UPDATE NO ACTION ON DELETE NO ACTION,
CONSTRAINT `fk_cat_imageid` FOREIGN KEY (`ImageID`) REFERENCES `image` (`ID`) ON UPDATE NO ACTION ON DELETE NO ACTION
)
COLLATE='utf8mb4_0900_ai_ci'
ENGINE=InnoDB
AUTO_INCREMENT=24
;


CREATE TABLE `instance` (
`ID` BIGINT(19) NOT NULL AUTO_INCREMENT,
`Name` VARCHAR(100) NOT NULL COLLATE 'utf8mb4_0900_ai_ci',
`FolderPath` VARCHAR(200) NOT NULL COLLATE 'utf8mb4_0900_ai_ci',
`ClientModpackPath` VARCHAR(200) NULL DEFAULT NULL COLLATE 'utf8mb4_0900_ai_ci',
`PortNumber` INT(10) NULL DEFAULT NULL,
`IconImageID` BIGINT(19) NULL DEFAULT NULL,
`IsSyncEnabled` TINYINT(3) NOT NULL,
`IsActive` TINYINT(3) NOT NULL,
PRIMARY KEY (`ID`) USING BTREE
)
COLLATE='utf8mb4_0900_ai_ci'
ENGINE=InnoDB
AUTO_INCREMENT=2
;


*/