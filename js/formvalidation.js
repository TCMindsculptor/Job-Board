var emailRegEx = new RegExp(/^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$/);

function validateForm(event) {
    //With custom JS we will require each field 
    var name = document.forms['main-contact-form']['name'].value;
    var email = document.forms['main-contact-form']['email'].value;
    var subject = document.forms['main-contact-form']['subject'].value;
    var message = document.forms['main-contact-form']['message'].value;

    //Get error message <span>
    var nameVal = document.getElementById('nameVal');
    var emailVal = document.getElementById('emailVal');
    var subjectVal = document.getElementById('subjectVal');
    var messageVal = document.getElementById('messageVal');

    //Test all of our conditions including checking for a valid email address
    if (name.length == 0 || email.length == 0 || subject.length == 0 || message.length == 0 || !emailRegEx.test(email)) {

        //Error messages under each required field 
        if (name.length == 0) {
            nameVal.textContent = "* Name is required.";
        } else {
            nameVal.textContent = "";
        }

        if (email.length == 0) {
            emailVal.textContent = "* Email is required.";
        } else {
            emailVal.textContent = "";
        }

        //Error Message if Email is not valid 
        if (!emailRegEx.test(email) && email.length > 0) {
            emailVal.textContent = "* Must be a valid email address.";
        }

        if (emailRegEx.test(email) && email.length > 0) {
            emailVal.textContent = "";
        }

        if (subject.length == 0) {
            subjectVal.textContent = "* Subject is required.";
        } else {
            subjectVal.textContent = "";
        }

        if (message.length == 0) {
            messageVal.textContent = "* Message is required.";
        } else {
            messageVal.textContent = "";
        }

        event.preventDefault();

    }
}