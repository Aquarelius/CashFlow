var utils = {
    ToPercentageString(value) {
        if (value || value === 0) {
            return value.toFixed(2) + " %";
        }
        return "-- %";
    },
    ToMoneyString(value, currency) {
        if (value || value === 0) {
            let res = value.toFixed(2);
            if (currency) res += " " + currency;
            return res;
        } else {
            return "--";
        }
    },
    ToShortDateString(value) {
        if (!value) return "";
        var dt = new Date(value);
        return dt.getDate() + "." + (dt.getMonth() +1);
    },
    ToPercentageForProgressBar(value) {
        if (value <= 100) return Math.round(value);
        return 100;

    },
    ProgressColorClass(value) {
        if (value < 100) return "cf-progress-good";
        return "cf-progress-bad";
    },
    TotalColorClass(value) {
        if (value < 0) return "cf-total-bad";
        return "cf-total-good";
    }
}
