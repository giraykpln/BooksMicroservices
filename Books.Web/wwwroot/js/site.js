"use strict";

// All requests are sent to the Books.API.
const API_URL = "https://localhost:7091/api";

async function send(method, path, body) {
    const response = await fetch(API_URL + path, {
        method: method,
        headers: { "Content-Type": "application/json" },
        body: body ? JSON.stringify(body) : undefined
    });
    if (response.status === 204)
        return [];
    const text = await response.text();
    const data = text ? JSON.parse(text) : null;
    if (!response.ok)
        throw new Error(data && data.message ? data.message.split("|").join(" ") : "An error occured!");
    return data;
}

function showMessage(text, isSuccessful) {
    const message = document.getElementById("message");
    message.textContent = text;
    message.className = "alert " + (isSuccessful ? "alert-success" : "alert-danger");
}

function cell(row, text) {
    const td = document.createElement("td");
    td.textContent = text;
    row.appendChild(td);
    return td;
}

async function loadBooks() {
    const books = await send("GET", "/books");
    const tbody = document.getElementById("books");
    tbody.replaceChildren();
    books.forEach(book => {
        const row = document.createElement("tr");
        cell(row, book.name);
        cell(row, book.authorNameR);
        cell(row, book.genreNamesR);
        cell(row, book.numberOfPagesR);
        cell(row, book.publishDateR);
        cell(row, book.priceR);
        cell(row, book.isTopSellerR);
        const td = cell(row, "");
        const editButton = document.createElement("button");
        editButton.className = "btn btn-sm btn-warning me-1";
        editButton.textContent = "Edit";
        editButton.onclick = () => editBook(book);
        const deleteButton = document.createElement("button");
        deleteButton.className = "btn btn-sm btn-danger";
        deleteButton.textContent = "Delete";
        deleteButton.onclick = () => deleteBook(book);
        td.append(editButton, deleteButton);
        tbody.appendChild(row);
    });
}

async function loadAuthorsAndGenres() {
    const authors = await send("GET", "/authors");
    const select = document.getElementById("authorId");
    select.replaceChildren(new Option("-- Select --", ""));
    authors.forEach(a => select.appendChild(new Option(a.fullNameR, a.id)));

    const genres = await send("GET", "/genres");
    const div = document.getElementById("genres");
    div.replaceChildren();
    genres.forEach(g => {
        const wrapper = document.createElement("div");
        wrapper.className = "form-check";
        const input = document.createElement("input");
        input.type = "checkbox";
        input.className = "form-check-input";
        input.id = "genre" + g.id;
        input.value = g.id;
        const label = document.createElement("label");
        label.className = "form-check-label";
        label.htmlFor = input.id;
        label.textContent = g.name;
        wrapper.append(input, label);
        div.appendChild(wrapper);
    });
}

function resetForm() {
    document.getElementById("bookForm").reset();
    document.getElementById("bookId").value = "";
    document.getElementById("bookFormTitle").textContent = "Add Book";
}

function editBook(book) {
    document.getElementById("bookId").value = book.id;
    document.getElementById("name").value = book.name;
    document.getElementById("authorId").value = book.authorId;
    document.getElementById("numberOfPages").value = book.numberOfPages ?? "";
    document.getElementById("publishDate").value = book.publishDate.substring(0, 10);
    document.getElementById("price").value = book.price;
    document.getElementById("isTopSeller").checked = book.isTopSeller;
    document.querySelectorAll("#genres input").forEach(input =>
        input.checked = book.genreIds.includes(Number(input.value)));
    document.getElementById("bookFormTitle").textContent = "Edit Book";
    window.scrollTo(0, 0);
}

async function deleteBook(book) {
    if (!confirm(book.name + " will be deleted. Are you sure?"))
        return;
    try {
        const response = await send("DELETE", "/books/" + book.id);
        showMessage(response.message, true);
        await loadBooks();
    } catch (error) {
        showMessage(error.message, false);
    }
}

async function saveBook(event) {
    event.preventDefault();
    const id = document.getElementById("bookId").value;
    const pages = document.getElementById("numberOfPages").value;
    const book = {
        name: document.getElementById("name").value,
        authorId: Number(document.getElementById("authorId").value),
        numberOfPages: pages ? Number(pages) : null,
        publishDate: document.getElementById("publishDate").value,
        price: Number(document.getElementById("price").value),
        isTopSeller: document.getElementById("isTopSeller").checked,
        genreIds: [...document.querySelectorAll("#genres input:checked")].map(i => Number(i.value))
    };
    try {
        let response;
        if (id) {
            book.id = Number(id);
            response = await send("PUT", "/books", book);
        } else {
            response = await send("POST", "/books", book);
        }
        showMessage(response.message, true);
        resetForm();
        await loadBooks();
    } catch (error) {
        showMessage(error.message, false);
    }
}

async function render() {
    try {
        await loadAuthorsAndGenres();
        await loadBooks();
    } catch (error) {
        showMessage(error.message, false);
    }
}

document.getElementById("bookForm").addEventListener("submit", saveBook);
document.getElementById("cancelButton").addEventListener("click", resetForm);
render();
