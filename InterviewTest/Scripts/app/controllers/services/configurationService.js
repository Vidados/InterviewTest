app.factory('configService', [
    '$http', function($http) {
        var configService = {}

        var _saveConfig = function(data) {
            return $http.post('http://localhost:61549/Newsletter/configure', data);
        }

        configService.saveConfig = _saveConfig;

        return configService;
    }
]);