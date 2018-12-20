/// <reference path="../Scripts/typings/jquery/jquery.d.ts"/>
/// <reference path="../Scripts/typings/AngularJS/angular.d.ts"/>


var app = angular.module("cashFlow", []);
 
app.controller("planState", ($scope, $http) => {
    $scope.utils = utils;
  
    $scope.planState = null;
    $scope.loading = true;
    $http.get("/api/Web/PlanState").then(data => {
        console.log(data.data.SecondCurrency);
        var ps = data.data.PlanState;
        $scope.loading = false;
        $scope.planState = ps;
        $scope.useSecondCur = data.data.SecondCurrency != null;
        $scope.secondCur = data.data.SecondCurrency;
        eval("Guage(" + ps.PlanLimit + "," + (ps.PlannedSpend - ps.PlanSpent) + ");");
    });
    $scope.setGuage = () => {
      
    }
});

app.controller("register", ($scope) => {
    
});

app.controller("login", ($scope) => {
    $scope.setEmail=(email) => {
        $scope.email = email;
    }
    $scope.email = "";
});

