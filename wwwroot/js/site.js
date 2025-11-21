class Base64 {
    static #textEncoder = new TextEncoder();
    static #textDecoder = new TextDecoder();

    // https://datatracker.ietf.org/doc/html/rfc4648#section-4
    static encode = (str) => btoa(String.fromCharCode(...Base64.#textEncoder.encode(str)));
    static decode = (str) => Base64.#textDecoder.decode(Uint8Array.from(atob(str), c => c.charCodeAt(0)));

    // https://datatracker.ietf.org/doc/html/rfc4648#section-5
    static encodeUrl = (str) => this.encode(str).replace(/\+/g, '-').replace(/\//g, '_');
    static decodeUrl = (str) => this.decode(str.replace(/\-/g, '+').replace(/\_/g, '/'));

    static jwtEncodeBody = (header, payload) => this.encodeUrl(JSON.stringify(header)) + '.' + this.encodeUrl(JSON.stringify(payload));
    static jwtDecodePayload = (jwt) => JSON.parse(this.decodeUrl(jwt.split('.')[1]));
}

document.addEventListener('submit', e => {
    const form = e.target;// as HTMLFormElement;
    if(form && form["id"] == "auth-form" && form instanceof HTMLFormElement) {
        e.preventDefault();
        const formData = new FormData(form);
        const login = formData.get("user-login");
        const password = formData.get("user-password");
        console.log(login, password);
        /*
        Д.З. Реалізувати перевірку введених даних у модальне вікно автентифікації
        на предмет порожнього вводу, вивести відповідні помилки засобами bootstrap-валідації
        аналогічно до підходу, реалізованому при розгляді теми "форми"
        */
        // RFC 7617   https://datatracker.ietf.org/doc/html/rfc7617
        const userPass = login + ':' + password;
        const basicCredentials = Base64.encode(userPass);
        fetch("/User/Authenticate", {
            method: 'GET',
            headers: {
                'Authorization': "Basic " + basicCredentials
            }
        }).then(r => {
            if (r.status >= 400) {
                r.text().then(t => {
                    document.getElementById("auth-error").innerText = t;
                });
            }
            else {
                window.location.reload();
            }
        });
    }
});

/*
Base64
"ABC" ->
    A       B        C
01000001 01000010 01000011
      |      |      |
010000 010100 001001 000011 --- з таблиці Base64:
   Q      U      J      D


Client(Browser)                     Server (Backend)
GET / ---------------------------------> /Home/Index 
Page  <----------- HTML(Index) --------- return View()
<script>
  fetch("/User/Authenticate") ----------> UserController.Authenticate()
  headers: {                              
    'Authorization': "Basic ..." -------> Request.Headers.Authorization            
  }                                       ....
  .then(r =>                            | Response.StatusCode = StatusCodes.Status401Unauthorized;
  r.text()<--401,Credentials rejected.--| return Content($"Credentials rejected."); 
*/