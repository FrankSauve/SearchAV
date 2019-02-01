function isLoggedIn() {
    console.log("Loggedin: " + localStorage.getItem('jwtToken'));
    
    if (localStorage.getItem('jwtToken')) {
        return true;
    }
    else {
        return false
    }
}

function getAuthToken() {
    return localStorage.getItem('jwtToken');
}

function setAuthToken(token: any) {
    localStorage.setItem('jwtToken', token);
}

function removeAuthToken() {
    localStorage.removeItem('jwtToken');
}

function getEmail() {
    return localStorage.getItem('email');
}

function setEmail(email: string) {
    localStorage.setItem('email', email);
}

function setProfilePicture(profilePicture: string) {
    localStorage.setItem('profilePicture', profilePicture);
}

function getProfilePicture() {
    return localStorage.getItem('profilePicture');
}

function setName(name: string) {
    localStorage.setItem('name', name);
}

function getName() {
    return localStorage.getItem('name');
}

export default{
    isLoggedIn: isLoggedIn,
    getAuthToken: getAuthToken,
    setAuthToken: setAuthToken,
    removeAuthToken: removeAuthToken,
    getEmail: getEmail,
    setEmail: setEmail,
    setProfilePicture: setProfilePicture,
    getProfilePicture: getProfilePicture,
    setName: setName,
    getName: getName
}