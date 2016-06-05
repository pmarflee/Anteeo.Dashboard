'use strict';

angular.module('dashboard', [
    'ngRoute',
    'ngAnimate',
    'ui.bootstrap',
    'ui-notification',
    'animate-change',
    'ng.epoch',
    'dashboard.services',
    'dashboard.directives',
    'dashboard.controllers'])
.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/', { templateUrl: 'app/views/dashboard.html', controller: 'DashboardCtrl' });
    $routeProvider.otherwise({ redirectTo: '/' });
}]);