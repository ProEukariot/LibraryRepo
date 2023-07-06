
const handlePwdVisibility = (target, id) => {
    const pwdControl = document.getElementById(id);

    if (pwdControl.type == "password") {
        pwdControl.type = "text";
        target.textContent = "visibility_off";
    } else {
        pwdControl.type = "password";
        target.textContent = "visibility";
    }
}

