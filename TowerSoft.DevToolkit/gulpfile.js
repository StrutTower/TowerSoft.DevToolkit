/// <binding ProjectOpened='sass-watch' />
var gulp = require('gulp'),
    concat = require('gulp-concat'),
    sass = require('gulp-dart-sass'),
    cleancss = require('gulp-clean-css'),
    rename = require('gulp-rename');

var options = {
    css: {
        libFiles: [],
        workingDirectory: 'node_modules',
        sassSource: 'Sass/site.scss',
        sassFiles: 'Sass/**/*.scss',
        output: 'bundle.css',
        dest: 'wwwroot/lib'
    },
    fonts: {
        files: [
            'node_modules/@mdi/font/fonts/*.*'
        ],
        css: 'node_modules/@mdi/font/css/materialdesignicons.css',
        dest: 'wwwroot/fonts'
    }
};

gulp.task('bundle-css', function () {
    //return gulp.src(options.css.libFiles)
    //    .pipe(gulp.src(options.css.sassSource)) // Swap first line to this if anything is added to the options.css.libFiles 
    return gulp.src(options.css.sassSource)
        .pipe(sass({
            errLogToConsole: true
        }).on('error', sass.logError))
        .pipe(concat(options.css.output))
        .pipe(gulp.dest(options.css.dest))
        .pipe(cleancss())
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(options.css.dest));
});

gulp.task('copy-fonts', function () {
    return gulp.src(options.fonts.files, { encoding: false })
        .pipe(gulp.dest(options.fonts.dest));
});

gulp.task('copy-font-css', function () {
    return gulp.src(options.fonts.css)
        .pipe(gulp.dest(options.css.dest))
        .pipe(cleancss())
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(options.css.dest));
})

gulp.task('sass-watch', function () {
    gulp.watch(options.css.sassFiles, gulp.parallel('bundle-css'));
});

gulp.task('default', gulp.parallel('bundle-css', 'copy-fonts', 'copy-font-css'));