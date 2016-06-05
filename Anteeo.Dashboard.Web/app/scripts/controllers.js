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
            },
            updateStatus = function (result) {
                var source = getSource(result),
                oldStatus,
                newStatus;

                switch (result.type) {
                    case "website":
                        oldStatus = source.status;
                        newStatus = result.status;
                        source.status = result.status;
                        source.message = result.message;
                        break;
                    case "database":
                        var database = source.databases.find(function (database) {
                            return database.name === result.name;
                        });
                        oldStatus = database.status;
                        newStatus = result.status;
                        database.status = result.status;
                        database.message = result.message;
                        break;
                }

                if (oldStatus !== newStatus) {
                    displayNotification(result);
                }
            },
            generateLineData = function () {
                var data1 = [{ label: 'Layer 1', values: [] }];
                for (var i = 0; i <= 128; i++) {
                    var x = 20 * (i / 128) - 10,
                        y = Math.cos(x) * x;
                    data1[0].values.push({ x: x, y: y });
                }
                var data2 = [
                    { label: 'Layer 1', values: [] },
                    { label: 'Layer 2', values: [] },
                    { label: 'Layer 3', values: [] }
                ];
                for (var i = 0; i < 256; i++) {
                    var x = 40 * (i / 256) - 20;
                    data2[0].values.push({ x: x, y: Math.sin(x) * (x / 4) });
                    data2[1].values.push({ x: x, y: Math.cos(x) * (x / Math.PI) });
                    data2[2].values.push({ x: x, y: Math.sin(x) * (x / 2) });
                }
                return data2;
            };

        var monitoringHub = backendHubProxy(backendHubProxy.defaultServer, "monitoringHub", { logging: true });

        monitoringHub.on("broadcastMonitoring", function (result) {
            $log.log(result);

            updateStatus(result);
        });

        monitoringHub.on("broadcastPerformanceMonitoring", function (result) {
            $log.log(result);

            var source = getSource(result),
                timestamp = ((new Date()).getTime() / 1000) | 0,
                chartEntry = [];


            chartEntry.push({ time: timestamp, y: result.value });

            source.percentProcessorTime = result.value;
            source.realtimePerformanceLineFeed = chartEntry;
        });

        monitoringHub.start(function () {
            monitoringHub.invoke("getDashboard", function (dashboard) {
                dashboard.environments.forEach(function (env) {
                    env.groups.forEach(function (group) {
                        group.sources.forEach(function (source) {
                            if (source.monitorCpu) {
                                source.realtimeLine = generateLineData();
                                source.options = { thickness: 10, mode: 'gauge', total: 100 };
                                source.lineAxes = ['right', 'bottom'];
                            }
                        });
                    });
                });
                $scope.dashboard = dashboard;
            });
        });
    }]);