'use strict';

angular.module('wubipawsApp.directives', [])

    .directive('appVersion', ['version', function (version) {
        return function (scope, elm, attrs) {
            elm.text(version);
        };
    }]);