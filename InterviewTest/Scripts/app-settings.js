function toggle(dropdown) {
    var val = $(dropdown).val();
    for (i = 0; i < val ; i++) {
        $("#item-ph" + i).show();
    }
    for (i = val; i < 10 ; i++) {
        $("#item-ph" + i).hide();
    }
}