/// <binding ProjectOpened='sass-watch' />
var gulp = require('gulp'),
    concat = require('gulp-concat'),
    sass = require('gulp-dart-sass'),
    lightningcss = require('gulp-lightningcss'),
    rename = require('gulp-rename');

var options = {
    css: {
        libFiles: [],
        workingDirectory: 'node_modules',
        sassSource: 'Sass/site.scss',
        sassFiles: 'Sass/**/*.scss',
        output: 'app.css',
        dest: 'wwwroot'
    }
};

gulp.task('bundle-css', function () {
    return gulp.src(options.css.sassSource)
        .pipe(sass({
            errLogToConsole: true
        }).on('error', sass.logError))
        .pipe(concat(options.css.output))
        .pipe(gulp.dest(options.css.dest))
        .pipe(lightningcss())
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest(options.css.dest));
});

gulp.task('sass-watch', function () {
    gulp.watch(options.css.sassFiles, gulp.parallel('bundle-css'));
});

gulp.task('default', gulp.parallel('bundle-css'));