app.controller('configCtrl', [ 'configService', '$window',
    function(configService, $window) {
        var vm = this;
        vm.configModel = [
            { id: "H", value: "Host" },
            { id: "T", value: "Trip" }
        ];
        vm.items = [ vm.configModel ];
        vm.selectedValues = [];

        vm.errorMsg = "Please select a value in all field or remove them.";

        vm.addConfigItem = function() {
            var newModel = angular.copy(vm.configModel);
            vm.items.push(newModel);
            console.log(vm.selectedValues);
        }

        vm.removeItem = function(item) {
            var index = vm.items.indexOf(item);
            vm.items.splice(index, 1);
            vm.selectedValues[index] = '';
        }

        vm.saveConfig = function () {
            configService.saveConfig({ configTokens: vm.selectedValues })
                .then(function (res) {
                    if (res.success === "true") {
                        $window.location.href='/newsletter/list';
                    }
                }, function(err) {
                    vm.errorMsg = err;
                });
        }

        vm.reset = function() {
            vm.items = [vm.configModel];
            vm.selectedValues = [];
        }

        vm.canSubmit = function() {
            return 
        };
    }
]);