var app = angular.module('dashboard.controllers', []);

app.controller('DashboardCtrl', ['$scope', '$log', 'backendHubProxy', 'Notification',
    function ($scope, $log, backendHubProxy, Notification) {
        $scope.dashboard = {};
        $scope.getPanelStatus = function (status) {
            return status ? 'panel-' + status : 'panel-default';
        };

        var getSource = function (result) {
            var environment = $scope.dashboard.environments.find(function (environment) {
                return environment.name === result.environment;
            });
            var group = environment.groups.find(function (group) {
                return group.name === result.group;
            });
            var source = group.sources.find(function (source) {
                return source.name === result.source;
            });

            return source;
        },
        displayNotification = function (result) {
            var message;

            switch (result.type) {
                case "website":
                    message = "Website: " + result.source + " - " + result.message;
                    break;
                case "database":
                    message = "Connection: " + result.source + "/" + result.name + " - " + result.message;
                    break;
            }

            switch (result.status) {
                case "success":
                    Notification.success(message);
                    break;
                case "warning":
                    Notification.warning(message);
                    break;
                case "danger":
                    Notification.error(message);
                    break;
                default:
                    Notification(message);
                    break;
            }
        };

        var monitoringHub = backendHubProxy(backendHubProxy.defaultServer, "monitoringHub", { logging: true });
        monitoringHub.on("broadcastMonitoring", function (result) {
            $log.log(result);

            var source = getSource(result);

            switch (result.type) {
                case "website":
                    source.status = result.status;
                    source.message = result.message;
                    break;
                case "database":
                    var database = source.databases.find(function (database) {
                        return database.name === result.name;
                    });
                    database.status = result.status;
                    database.message = result.message;
                    break;
            }

            displayNotification(result);
        });
        monitoringHub.start(function () {
            monitoringHub.invoke("getDashboard", function (dashboard) {
                $scope.dashboard = dashboard;
            });
        });
    }]);