var StipendiumApp = angular.module('StipendiumApp', []);



StipendiumApp.controller('StipendiumController', function ($scope, StipendiumService) {

    $scope.title = "Stipendium Users";

    getStiftelser();

    function getStiftelser() {
        StipendiumService.getStiftelses().then(function (input) {
            $scope.stiftelse = input.data;
            console.log(input);
        }), function (error) {
            $scope.status = 'Unable to load customer data: ' + error.message;
            console.log("Error");
        };
    }
});

StipendiumApp.controller('UsersController', function ($scope, StipendiumService) {
    getUsers();

    function getUsers() {
        StipendiumService.getUsers().then(function (input) {
            $scope.users = input.data;
            console.log(input);
        }), function (error) {
            $scope.status = 'Unable to load customer data: ' + error.message;
            console.log("Error");
        };
    }
});

StipendiumApp.factory('StipendiumService', ['$http', function ($http) {

    var StipendiumService = {};

    StipendiumService.getStiftelses = function () {

        return $http.get('/Admin/GetPopularStiftelses');

    };

    StipendiumService.getUsers = function () {

        return $http.get('/Admin/GetIdleUsers');

    };

    return StipendiumService;

}]);

