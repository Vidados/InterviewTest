app.factory('configService', [
    '$http', function($http) {
        var configService = {}

        var _saveConfig = function(data) {
            return $http.post('http://localhost:61549/Newsletter/configure', data)
                .then(function(res) {
                    return res.data;
                }, function(err) {
                    return err;
                });
        }

        configService.saveConfig = _saveConfig;

        return configService;
    }
]);