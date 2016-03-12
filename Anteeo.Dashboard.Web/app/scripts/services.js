'use strict';

var services = angular.module('dashboard.services', ['ngResource']);

services.factory('backendHubProxy', ['$rootScope', function ($rootScope) {
    function backendHubProxyFactory(serverUrl, hubName, startOptions) {
        var connection = $.hubConnection(serverUrl),
            proxy = connection.createHubProxy(hubName);

        if (startOptions && startOptions.logging) {
            connection.logging = startOptions.logging;
        }

        return {
            on: function (eventName, callback) {
                proxy.on(eventName, function (result) {
                    $rootScope.$apply(function () {
                        if (callback) {
                            callback(result);
                        }
                    });
                });
            },
            invoke: function (methodName, callback) {
                proxy.invoke(methodName)
                    .done(function (result) {
                        $rootScope.$apply(function () {
                            if (callback) {
                                callback(result);
                            }
                        });
                    });
            },
            start: function (onInitCallback) {
                connection.start(startOptions).done(function () {
                    if (onInitCallback) {
                        onInitCallback();
                    }
                });
            }
        };
    };

    return backendHubProxyFactory;
}]);