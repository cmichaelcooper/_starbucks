var gulp = require('gulp');
var watch = require('gulp-watch');
var compass = require('gulp-compass');
var livereload = require('gulp-livereload');

gulp.task('compass', function() {
    gulp.src('css/src/*.scss')
    .pipe(compass({
        config_file: 'css/src/config.rb',
        css: 'css',
        sass: 'css/src',
        image: 'img',
        comments: 'true'
    }))
    .pipe(gulp.dest('css'));
    // .pipe(livereload());
});



gulp.task('default', function() {
    livereload.listen();
    gulp.watch('css/src/*.scss', ['compass']);
    gulp.watch('css/src/**').on('change', livereload.changed);
});