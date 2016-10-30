(function(global) {

    // map tells the System loader where to look for things
    var map = {
        'app': 'app', // 'dist',

        '@angular/core' : 'node_modules/@angular/core/bundles/core.umd.js',
        '@angular/common' : 'node_modules/@angular/common/bundles/common.umd.js',
        '@angular/compiler' : 'node_modules/@angular/compiler/bundles/compiler.umd.js',
        '@angular/platform-browser' : 'node_modules/@angular/platform-browser/bundles/platform-browser.umd.js',
        '@angular/platform-browser-dynamic' : 'node_modules/@angular/platform-browser-dynamic/bundles/platform-browser-dynamic.umd.js',
        '@angular/http' : 'node_modules/@angular/http/bundles/http.umd.js',
        '@angular/router' : 'node_modules/@angular/router/bundles/router.umd.js',
        '@angular/forms' : 'node_modules/@angular/forms/bundles/forms.umd.js',

        'rxjs': 'node_modules/rxjs',
        '@angular': 'node_modules/@angular',
        'moment': 'node_modules/moment',
        'codemirror': 'node_modules/codemirror',
        'lodash': 'node_modules/lodash',
        'node-uuid': 'node_modules/node-uuid',
        'angular2-mdl': 'node_modules/angular2-mdl',
        '@vaadin/angular2-polymer': 'node_modules/@vaadin/angular2-polymer',
        'angular2-data-table': 'node_modules/angular2-data-table',
        'ng2-handsontable': 'node_modules/ng2-handsontable'
    };

    // packages tells the System loader how to load when no filename and/or no extension
    var packages = {
        'app': { main: 'main.js',  defaultExtension: 'js' },
        'rxjs': { defaultExtension: 'js' },
        'moment': { main: 'moment.js', defaultExtension: 'js' },
        'codemirror': { main: 'lib/codemirror.js', defaultExtension: 'js' },
        'lodash': { main: 'lodash.js', defaultExtension: 'js' },
        'node-uuid': { main: 'uuid.js', defaultExtension: 'js' },
        'angular2-mdl': { main: 'bundle/angular2-mdl.js' },
        '@vaadin/angular2-polymer': { main: 'index.js' },
        'angular2-data-table': { main: 'release/index.js' },
        'ng2-handsontable': { main: 'dist/index.js', defaultExtension: 'js' }
    };

    var config = {
        map: map,
        packages: packages
    }

    System.config(config);

})(this);
