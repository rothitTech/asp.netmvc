/**
 * This type-detection plug-in will sort columns-with-numbers-and-empty-cells
 */


jQuery.fn.dataTableExt.oSort['numeric-null-asc'] = function (a, b) {
    var x = parseInt(a);
    var y = parseInt(b);
    return ((isNaN(y) || x < y) ? -1 : ((isNaN(x) || x > y) ? 1 : 0));
};
jQuery.fn.dataTableExt.oSort['numeric-null-desc'] = function (a, b) {
    var x = parseInt(a);
    var y = parseInt(b);
    return ((isNaN(y) || x < y) ? 1 : ((isNaN(x) || x > y) ? -1 : 0));
};