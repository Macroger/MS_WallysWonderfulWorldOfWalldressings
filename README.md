# MS_WallysWonderfulWorldOfWalldressings

This project is the solution to an assignment I had in the course Relational Databases, part of the SET program I took at Conestoga College Sept. 2017 - April 2020.

This project was built up over multiple assignments, it was designed to simulate a business contracting out the student to build a POS (Point of Sale) system that connects with a
MySQL database. Since this was a database course the emphasis was on the database design and implementation for this system. The first stages of this project had the student
develop an Entity Relationship Diagram and database schema (in 3rd normal form) to model the business operations that are relevant to database design. Assignment 2 
involved creating a function database from the schema created in assignment 1. Assignment 3 involved creating the front end for the databse, the POS system that 
the fictional business owner was requesting. This POS system was to interface with the database already to provide functions such as customer tracking, inventory checking of
other stores, and of course placing orders. 

This is my solution to the given problems / assignments. This system is capable of tracking multiple stores, a list of clients, and products / inventories. The entities tracked
by the database system have relationships and its possible to see what orders were placed at which stores, and the inventory levels for that individual store is tracked. 
Customers have the ability to return stock with inventory levels correctly reflected - a record is kept of the return and total sales / profit is adjusted. It communicates
with a MySQL server and utilzes C# / .NET Framework in order to provide the frontend UI.
