const MIN_USERNAME_LENGTH = 1;
const MIN_LOGIN_LENGTH = 5;
const MIN_PASSWORD_LENGHT = 8;
const MAX_USERNAME_LENGTH = 20;
const MAX_LOGIN_LENGTH = 20;
const MAX_PASSWORD_LENGHT = 20;

const isContainsLowerCase = (string) => /.*[a-z].*/.test(string);
const isContainsUpperCase = (string) => /.*[A-Z].*/.test(string);
const isContainsNotLetters = (string) => /.*[^a-zA-Z].*/.test(string);
const isConstainsSpaces = (string) => /.*\s.*/.test(string);


function validateRegistrationForm() {
    return isUsernameValid() && isLoginValid() && isPasswordValid() && isPasswordSame();
}

function isUsernameValid() {
    let username = document.getElementById("MainContent_Username").value;

    if (username.length < MIN_USERNAME_LENGTH) {
        showError(`имя пользователя меньше ${MIN_USERNAME_LENGTH} символов!`);
        return false;
    } else if (username.length > MAX_USERNAME_LENGTH) {
        showError(`имя пользователя больше ${MAX_USERNAME_LENGTH} символов!`);
        return false;
    } else if (isConstainsSpaces(username)) {
        showError(`имя пользователя не должно содержать пробелы!`);
        return false;
    } else {
        return true;
    }
}

function isLoginValid() {
    let login = document.getElementById("MainContent_Login").value;

    if (login.length < MIN_LOGIN_LENGTH) {
        showError(`логин меньше ${MIN_LOGIN_LENGTH} символов!`);
        return false;
    } else if (login.length > MAX_LOGIN_LENGTH) {
        showError(`логин больше ${MAX_LOGIN_LENGTH} символов!`);
        return false;
    } else if (!isContainsLowerCase(login) ||
        !isContainsUpperCase(login) ||
        !isContainsNotLetters(login) ||
        isConstainsSpaces(login)
    ) {
        showError("логин должен содержать хотя бы 1 строчную и заглавную буквы, хотя бы 1 не буквенный символ, и не должен содержать пробелы!");
        return false;
    } else {
        return true;
    }
}

function isPasswordValid() {
    let password = document.getElementById("MainContent_Password").value;

    if (password.length < MIN_PASSWORD_LENGHT) {
        showError(`пароль меньше ${MIN_PASSWORD_LENGHT} символов!`);
        return false;
    } else if (password.length > MAX_PASSWORD_LENGHT) {
        showError(`пароль больше ${MAX_PASSWORD_LENGHT} символов!`);
        return false;
    } else if (!isContainsLowerCase(password) ||
        !isContainsUpperCase(password) ||
        !isContainsNotLetters(password) ||
        isConstainsSpaces(password)
    ) {
        showError("пароль должен содержать хотя бы 1 строчную и заглавную буквы, хотя бы 1 не буквенный символ, и не должен содержать пробелы!");
        return false;
    } else {
        return true;
    }
}

function isPasswordSame() {
    let password = document.getElementById("MainContent_Password").value;
    let passwordConfirmed = document.getElementById("MainContent_PasswordConfirm").value;

    if (password != passwordConfirmed) {
        showError("пароли не совпадают!");
        return false;
    } else {
        return true;
    }
}

function showError(message) {
    let errorLabel = document.getElementById("MainContent_ErrorLabel");

    errorLabel.style.color = "Red";
    errorLabel.textContent = "Ошибка: " + message;
    errorLabel.style.display = "block";
}
