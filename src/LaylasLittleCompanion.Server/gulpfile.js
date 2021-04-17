const gulp = require('gulp');
const sass = require('gulp-sass');
var rename = require('gulp-rename');
const del = require('del');

//https://www.npmjs.com/package/gulp-rename

gulp.task('styles', () => {
	return gulp.src('Styles/scss/*.scss')
		.pipe(sass().on('error', sass.logError))
		.pipe(rename(function (path) {
			// Updates the object in-place
			path.dirname += "/css";
			//path.basename += "-goodbye";
			//path.extname = ".md";
		}))
		.pipe(gulp.dest('././wwwroot'));
});

//gulp.task('copy-fonts', () => {
//	return gulp.src('././node_modules/@patternfly/patternfly/assets/fonts/**/*')
//		//.pipe(rename(function (path) {
//		//	path.dirname += "";
//		//}))
//		.pipe(gulp.dest('./wwwroot/css/assets/fonts'));
//})




gulp.task('default', gulp.series(['styles',]));