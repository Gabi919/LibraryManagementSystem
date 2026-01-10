DROP TABLE IF EXISTS Loans;
DROP TABLE IF EXISTS Books;
DROP TABLE IF EXISTS Users;
DROP TABLE IF EXISTS Categories;
DROP TABLE IF EXISTS Roles;

CREATE TABLE Roles (
    RoleId SERIAL PRIMARY KEY,
    RoleName VARCHAR(50) NOT NULL
);

CREATE TABLE Categories (
    CategoryId SERIAL PRIMARY KEY,
    CategoryName VARCHAR(100) NOT NULL
);

CREATE TABLE Users (
    UserId SERIAL PRIMARY KEY,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(150) UNIQUE NOT NULL,
    Phone VARCHAR(20),
    RegisteredDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    RoleId INT REFERENCES Roles(RoleId)
);

CREATE TABLE Books (
    BookId SERIAL PRIMARY KEY,
    Title VARCHAR(200) NOT NULL,
    Author VARCHAR(150) NOT NULL,
    Isbn VARCHAR(20),
    Publisher VARCHAR(100),
    PublishedYear INT,
    TotalStock INT NOT NULL DEFAULT 0,
    CurrentStock INT NOT NULL DEFAULT 0,
    CategoryId INT REFERENCES Categories(CategoryId)
);

CREATE TABLE Loans (
    LoanId SERIAL PRIMARY KEY,
    UserId INT REFERENCES Users(UserId),
    BookId INT REFERENCES Books(BookId),
    LoanDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    DueDate TIMESTAMP NOT NULL,
    ReturnDate TIMESTAMP
);

INSERT INTO Roles (RoleName) VALUES ('Administrator'), ('Member');

INSERT INTO Categories (CategoryName) VALUES ('Science Fiction'), ('History'), ('Technology'), ('Romance');

INSERT INTO Users (FirstName, LastName, Email, Phone, RoleId) VALUES 
('Ion', 'Popescu', 'ion.popescu@email.com', '0722123456', 2),
('Maria', 'Ionescu', 'maria.ionescu@email.com', '0744987654', 2),
('Admin', 'Sistem', 'admin@library.com', NULL, 1);

INSERT INTO Books (Title, Author, Isbn, Publisher, PublishedYear, TotalStock, CurrentStock, CategoryId) VALUES 
('Dune', 'Frank Herbert', '978-0441013593', 'Ace', 1965, 5, 5, 1),
('Sapiens: Scurta istorie a omenirii', 'Yuval Noah Harari', '978-0062316097', 'Harper', 2015, 3, 2, 2),
('Clean Code', 'Robert C. Martin', '978-0132350884', 'Pearson', 2008, 10, 10, 3),
('Foundation', 'Isaac Asimov', '978-0553293357', 'Bantam', 1951, 4, 4, 1);

INSERT INTO Loans (UserId, BookId, LoanDate, DueDate, ReturnDate) VALUES
(2, 2, CURRENT_TIMESTAMP - INTERVAL '5 days', CURRENT_TIMESTAMP + INTERVAL '9 days', NULL);