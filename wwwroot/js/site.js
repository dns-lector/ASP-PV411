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
    const form = e.target;
    if (form && form["id"] == "signup-form" && form instanceof HTMLFormElement) {
        e.preventDefault();              // Перехоплення надсилання форми та 
        fetch("/User/Register", {        // переведення його до асинхронної
            method: "POST",              // форми (AJAX)
            body: new FormData(form)     // 
        }).then(r => {                   // 
            return r.json();
        }).then(j => {
            if (j.status == "Ok") {
                alert("Вітаємо з успішною реєстрацією!");
            }
            else {

                for (let elem of form.querySelectorAll("input")) {
                    elem.classList.remove("is-invalid");
                }

                for (let name in j['errors']) {
                    let input = form.querySelector(`[name="${name}"]`);
                    if (input) {
                        input.classList.add("is-invalid");
                        let fb = form.querySelector(`[name="${name}"]+.invalid-feedback`);
                        if (fb) {
                            fb.innerText = j['errors'][name];
                        }
                    }
                    else {
                        console.error(`Input name = '${name}' not found`);
                    }
                }
            }
        });
        /*
        Д.З. Завершити виведення повідомлень валідації полів форми 
        реєстрації нового користувача.
        За аналогією FormModels додати валідацію паролю на вміст
        різного контенту, а також валідацію е-пошти за стандартним шаблоном
        * Реалізувати виділення полів, що пройшли валідацію, "зеленим" стилем
        * Додати валідацію телефона
        */
    }

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

document.addEventListener('DOMContentLoaded', () => {
    let btn = document.getElementById("btn-profile-edit");
    if (btn) btn.addEventListener('click', btnProfileEditClick);

    btn = document.getElementById("btn-profile-delete");
    if (btn) btn.addEventListener('click', btnProfileDeleteClick);
});

function btnProfileDeleteClick() {
    if (confirm("Ви збираєтесь закрити профіль. Уся персональна інформація буде видалена і не підлягатиме відновленню. Підтверджуєте?")) {
        fetch("/User/Erase", {
            method: "DELETE"
        }).then(r => {
            if (r.ok) {
                alert("Ваш профіль видалено.");
                window.location = "/";
            }
            else {
                alert("Виникла помилка, повторіть спробу пізніше");
            }
        });
    }
}

function btnProfileEditClick(e) {
    const btn = e.target.closest("button");
    if (btn.isActivated) {
        btn.isActivated = false;
        btn.classList.remove("btn-outline-success");
        btn.classList.add("btn-outline-warning");
        let changes = {};
        for (let item of document.querySelectorAll("[data-profile-editable]")) {
            item.removeAttribute("contenteditable");
            let currentText = item.innerText == '\n' ? '' : item.innerText;
            if (item.initialText != currentText) {
                changes[item.getAttribute("data-profile-editable")] = currentText;
            }
            let tr = item.closest("[data-profile-hidden]");
            if (tr) {
                tr.style.display = "none";
            }
        }
        if (Object.keys(changes).length > 0) {
            console.log(changes);
            fetch("/user/update", {
                method: "PATCH",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(changes)
            }).then(r => {
                if (r.status == 202) {
                    window.location.reload();
                }
                else {
                    r.text().then(alert);
                }
            });
        }

    }
    else {
        btn.isActivated = true;
        btn.classList.remove("btn-outline-warning");
        btn.classList.add("btn-outline-success");
        // console.log(document.querySelectorAll("[data-profile-editable]"));
        for (let item of document.querySelectorAll("[data-profile-editable]")) {
            item.setAttribute("contenteditable", true);
            item.initialText = item.innerText;
            let tr = item.closest("[data-profile-hidden]");
            if (tr) {
                tr.style.display = "table-row";
            }
        }

    }
}

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