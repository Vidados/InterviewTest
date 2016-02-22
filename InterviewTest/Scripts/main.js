var allowedChars = [72, 84, 104, 116];

$(document).ready(function()
{
    $("#txtNewsLetterFormat").keypress(function (e)
    {
        console.log(e.which);
        if ($.inArray(e.which, allowedChars) == -1)
        {
            e.preventDefault();
            return false;
        }
    });

    $("#txtNewsLetterFormat").keyup(function(e)
    {
        $("#txtNewsLetterFormat").val($("#txtNewsLetterFormat").val().toUpperCase());
    });
});

function validateSubmit()
{
    var numOfNewsletters = $("#txtNumberOfNewsletters").val();
    if (numOfNewsletters == "" || parseInt(numOfNewsletters) < 1)
    {
        alert("Please select the number of Newsletters to create");
        return false;
    }

    if ($("#txtNewsLetterFormat").val().length == 0)
    {
        alert("Please enter a format for the newsletter. Example: TTHH");
        return false;
    }

    return true;
}