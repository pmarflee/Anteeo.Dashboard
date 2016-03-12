var app = angular.module('dashboard.controllers', []);

app.controller('DashboardCtrl', ['$scope', '$log', 'backendHubProxy',
    function ($scope, $log, backendHubProxy) {
        $scope.dashboard = {};
        $scope.getPanelStatus = function (status) {
            return status ? 'panel-' + status : 'panel-default';
        };

        var monitoringHub = backendHubProxy(backendHubProxy.defaultServer, "monitoringHub", { logging: true });
        monitoringHub.on("broadcastMonitoring", function (result) {
            $log.log(result);
            var environment = $scope.dashboard.environments.find(function (environment) {
                return environment.name === result.environment;
            });
            var group = environment.groups.find(function (group) {
                return group.name === result.group;
            });
            var source = group.sources.find(function (source) {
                return source.name === result.source;
            });
            switch (result.type) {
                case "website":
                    source.status = result.status;
                    break;
                case "database":
                    var database = source.databases.find(function (database) {
                        return database.name === result.name;
                    });
                    database.status = result.status;
                    break;
            }
        });
        monitoringHub.start(function () {
            monitoringHub.invoke("getDashboard", function (dashboard) {
                $scope.dashboard = dashboard;
            });
        });
    }]);