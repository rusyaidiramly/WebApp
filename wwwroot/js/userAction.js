async function postData(url = "", data = {}) {
    // Default options are marked with *
    const response = await fetch(url, {
        method: "POST", // *GET, POST, PUT, DELETE, etc.
        mode: "cors", // no-cors, *cors, same-origin
        cache: "no-cache", // *default, no-cache, reload, force-cache, only-if-cached
        credentials: "same-origin", // include, *same-origin, omit
        headers: {
            "Content-Type": "application/json",
            // "Content-Type": "application/x-www-form-urlencoded",
        },
        redirect: "follow", // manual, *follow, error
        referrerPolicy: "no-referrer", // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
        body: JSON.stringify(data), // body data type must match "Content-Type" header
    });
    return response.json(); // parses JSON response into native JavaScript objects
}

async function editUser(url = "", data = {}) {
    // var formBody = [];
    // for (var property in data) {
    //     var encodedKey = encodeURIComponent(property);
    //     var encodedValue = encodeURIComponent(data[property]);
    //     formBody.push(encodedKey + "=" + encodedValue);
    // }
    // formBody = formBody.join("&");
    const response = await fetch(url, {
        method: "PUT",
        mode: "cors",
        cache: "no-cache",
        credentials: "same-origin",
        headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
            // "Content-Type": "application/x-www-form-urlencoded",
            url: url,
        },
        redirect: "follow",
        referrerPolicy: "no-referrer",
        body: JSON.stringify(data),
    });
    return response.json();
}

async function deleteUser(url = "") {
    const response = await fetch(url, {
        method: "DELETE",
        mode: "cors",
        cache: "no-cache",
        credentials: "same-origin",
        headers: {
            "Content-Type": "application/json",
        },
        redirect: "follow",
        referrerPolicy: "no-referrer",
    });
    return response.json();
}

window.onload = function () {
    editBtns = document.querySelectorAll(".action-edit");
    deleteBtns = document.querySelectorAll(".action-delete");
    addBtn = document.getElementById("add-new-user");

    addBtn.addEventListener("click", function (e) {
        e.preventDefault();
        tbody = document.getElementById("table-body-userlist");
        Swal.fire({
            title: `Add user`,
            showDenyButton: false,
            showCancelButton: true,
            confirmButtonText: "Save",
            confirmButtonColor: "#198754",
            html: `<div class="container">
                <form>
                <div class="form-group mb-2">
                    <label for="swal-input-email" style="float:left">Email</label>
                    <input type="email" class="form-control" id="swal-input-email" placeholder="Email">
                </div>
                <div class="form-group mb-2">
                    <label for="swal-input-name" style="float:left">Full name</label>
                    <input type="text" class="form-control" id="swal-input-name" placeholder="Name">
                </div>
                <div class="form-group mb-2">
                    <label for="swal-input-nric" style="float:left">IC No.</label>
                    <input type="text" class="form-control" id="swal-input-nric" placeholder="IC No.">
                </div>
                <div class="form-group mb-2">
                    <label for="swal-input-dob" style="float:left">Birth date</label>
                    <input type="text" class="form-control" id="swal-input-dob" placeholder="Birth date">
                </div>
                <div class="form-group mb-2">
                    <label for="swal-input-password" style="float:left">Password</label>
                    <input type="text" class="form-control" id="swal-input-password" placeholder="Password">
                </div>
                </form>
                </div>`,
            preConfirm: () => {
                return {
                    name: document.getElementById("swal-input-name").value,
                    email: document.getElementById("swal-input-email").value,
                    nric: document.getElementById("swal-input-nric").value,
                    dob: document.getElementById("swal-input-dob").value,
                    password: document.getElementById("swal-input-password")
                        .value,
                };
            },
        }).then((result) => {
            if (result.isConfirmed) {
                postData(`/api/user`, result.value).then((data) => {
                    if (data.success) {
                        Swal.fire(data.message, "", "success").then(
                            (result) => {
                                if (result.isConfirmed) location.reload();
                            }
                        );
                    } else {
                        Swal.fire(data.message, "", "info");
                    }
                });
            }
        });
    });

    editBtns.forEach((editBtn) => {
        editBtn.addEventListener("click", function (e) {
            e.preventDefault();
            userId = this.getAttribute("data-id");
            rowElement = this.closest("tr");
            userData = rowElement.children;
            Swal.fire({
                title: `Edit user: ${userId}`,
                showDenyButton: false,
                showCancelButton: true,
                confirmButtonText: "Save",
                confirmButtonColor: "#198754",
                html: `<div class="container">
                <form>
                <div class="form-group mb-2">
                    <label for="swal-input-email" style="float:left">Email</label>
                    <input type="email" class="form-control" id="swal-input-email" value="${userData[2].innerHTML}" placeholder="Email">
                </div>
                <div class="form-group mb-2">
                    <label for="swal-input-name" style="float:left">Full name</label>
                    <input type="text" class="form-control" id="swal-input-name"  value="${userData[1].innerHTML}" placeholder="Name">
                </div>
                <div class="form-group mb-2">
                    <label for="swal-input-nric" style="float:left">IC No.</label>
                    <input type="text" class="form-control" id="swal-input-nric"  value="${userData[4].innerHTML}" placeholder="IC No.">
                </div>
                </form>
                </div>`,
                preConfirm: () => {
                    return {
                        name: document.getElementById("swal-input-name").value,
                        email: document.getElementById("swal-input-email")
                            .value,
                        nric: document.getElementById("swal-input-nric").value,
                    };
                },
            }).then((result) => {
                if (result.isConfirmed) {
                    editUser(`/api/user/${userId}`, result.value).then(
                        (data) => {
                            if (data.success) {
                                userData[1].innerHTML = data.body.name;
                                userData[2].innerHTML = data.body.email;
                                userData[3].innerHTML = data.body.age;
                                userData[4].innerHTML = data.body.nric;
                                userData[5].innerHTML = data.body.dob;
                                Swal.fire(`Edit Saved`, "", "success");
                            }
                        }
                    );
                }
            });
        });
    });

    deleteBtns.forEach((deleteBtn) => {
        deleteBtn.addEventListener("click", function (e) {
            e.preventDefault();
            userId = this.getAttribute("data-id");
            rowElement = this.closest("tr");
            userData = rowElement.children;
            Swal.fire({
                icon: "warning",
                title: `Delete user: ${userId}`,
                showDenyButton: false,
                showCancelButton: true,
                confirmButtonText: "Delete",
                confirmButtonColor: "#d33",
                timer: 5000,
                timerProgressBar: true,
                denyButtonText: ``,
                html: `<table class="mx-auto" style="text-align:left">
                        <tr><td class="pe-2"><strong>Full Name:</strong></td><td>${userData[1].innerHTML}</td></tr>
                        <tr><td class="pe-2"><strong>Email Address:</strong></td><td>${userData[2].innerHTML}</td></tr></table>`,
            }).then((result) => {
                if (result.isConfirmed) {
                    deleteUser(`/api/user/${userId}`).then((data) => {
                        if (data.success) {
                            rowElement.remove();
                            Swal.fire(`Delete Success`, "", "success");
                        }
                    });
                }
            });
        });
    });
};

//   postData('https://example.com/answer', { answer: 42 })
//   .then(data => {
//     console.log(data); // JSON data parsed by `data.json()` call
//   });
