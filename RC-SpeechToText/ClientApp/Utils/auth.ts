function isLoggedIn() {
    console.log("Loggedin: " + localStorage.getItem('jwtToken'))
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

export default{
    isLoggedIn: isLoggedIn,
    getAuthToken: getAuthToken,
    setAuthToken: setAuthToken,
    removeAuthToken: removeAuthToken    
}