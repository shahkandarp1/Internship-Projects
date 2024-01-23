
use kandarpshah_db;
--1 write a SQL query to find the salesperson and customer who reside in the same city. Return Salesman, cust_name and city
select * from salesman as s inner join customer as c on s.salesman_id = s.salesman_id where s.city = c.city;

--2 Write a SQL query to find those orders where the order amount exists between 500 and 2000. Return ord_no, purch_amt, cust_name, city
select c.cust_name,c.city,o.ord_date,o.pur_amt from customer as c inner join orders as o on o.customer_id = c.customer_id where o.pur_amt between 500 and 2000;

--3 Write a SQL query to find the salesperson(s) and the customer(s) he represents. Return Customer Name, city, Salesman, commission
select c.cust_name,c.city,s.name,s.commission from customer as c right join salesman as s on c.salesman_id = s.salesman_id;

--4 Write a SQL query to find salespeople who received commissions of more than 12 percent from the company. Return Customer Name, customer city, Salesman, commission.
select c.cust_name,c.city,s.name,s.commission from customer as c inner join salesman as s on c.salesman_id = s.salesman_id where s.commission>12;

--5 Write a SQL query to locate those salespeople who do not live in the same city where their customers live and have received a commission of more than 12% from the company. Return Customer Name, customer city, Salesman, salesman city, commission
select c.cust_name,c.city,s.name,s.commission from customer as c inner join salesman as s on c.salesman_id = s.salesman_id and c.city != s.city where s.commission>12;

--6 Write a SQL query to find the details of an order. Return ord_no, ord_date, purch_amt, Customer Name, grade, Salesman, commission
select o.ord_no,o.ord_date,o.pur_amt,c.cust_name,c.grade,s.name,s.commission from customer as c inner join salesman as s on s.salesman_id = c.salesman_id inner join orders as o on c.customer_id = o.customer_id ;

--7 Write a SQL statement to join the tables salesman, customer and orders so that the same column of each table appears once and only the relational rows are returned.
select o.ord_no,o.ord_date,o.pur_amt,c.cust_name,c.grade,c.city,c.customer_id,s.name,s.commission,s.salesman_id,s.city from customer as c inner join salesman as s on s.salesman_id = c.salesman_id inner join orders as o on c.customer_id = o.customer_id ;

--8 write a SQL query to display the customer name, customer city, grade, salesman, salesman city. The results should be sorted by ascending customer_id.
select c.cust_name,c.grade,c.city,s.name,s.city from customer as c inner join salesman as s on s.salesman_id = c.salesman_id order by c.customer_id;

--9 write a SQL query to find those customers with a grade less than 300. Return cust_name, customer city, grade, Salesman, salesmancity. The result should be ordered by ascending customer_id.
select c.cust_name,c.grade,c.city,s.name,s.city from customer as c inner join salesman as s on s.salesman_id = c.salesman_id where c.grade < 300 order by c.customer_id;

--10 Write a SQL statement to make a report with customer name, city, order number, order date, and order amount in ascending order according to the order date to determine whether any of the existing customers have placed an order or not
select c.cust_name,c.city,o.ord_no,o.ord_date,CASE when o.ord_no is not null then 'Placed' else 'Not Placed' end as Order_placed from customer as c left join orders as o on c.customer_id = o.customer_id order by o.ord_date;

--11  Write a SQL statement to generate a report with customer name, city, order number, order date, order amount, salesperson name, and commission to determine if any of the existing customers have not placed orders or if they have placed orders through their salesman or by themselves
select o.ord_no,o.ord_date,o.pur_amt,c.cust_name,c.city,s.name,s.commission,CASE when o.ord_no is not null then 'Placed' else 'Not Placed' end as Order_placed,CASE when s.name is not null then 'Salesman' else 'Themselves' end as Placed_through from customer as c left join salesman as s on s.salesman_id = c.salesman_id  left join orders as o on c.customer_id = o.customer_id;

--12   Write a SQL statement to generate a list in ascending order of salespersons who work either for one or more customers or have not yet joined any of the customer
select s.name,CASE when c.salesman_id is not null then 'Has a Customer' else 'Does not have customer' end as Status from salesman as s left join customer as c on s.salesman_id = c.salesman_id;

--13 write a SQL query to list all salespersons along with customer name, city, grade, order number, date, and amount.
select o.ord_no,o.ord_date,o.pur_amt,c.cust_name,c.grade,c.city,s.name from customer as c right join salesman as s on s.salesman_id = c.salesman_id left join orders as o on c.customer_id = o.customer_id ;

--14 Write a SQL statement to make a list for the salesmen who either work for one or more customers or yet to join any of the customers. The customer may have placed, either one or more orders on or above order amount 2000 and must have a grade, or he may not have placed any order to the associated supplier
select * from salesman as s left join customer as c on s.salesman_id = c.salesman_id left join orders as o on c.customer_id = o.customer_id where (o.ord_no is NULL) or (o.ord_no is not NULL and o.pur_amt > 2000 and c.grade is not NULL);

--15  Write a SQL statement to generate a list of all the salesmen who either work for one or more customers or have yet to join any of them. The customer may have placed one or more orders at or above order amount 2000, and must have a grade, or he may not have placed any orders to the associated supplier.
select * from salesman as s left join customer as c on s.salesman_id = c.salesman_id left join orders as o on c.customer_id = o.customer_id where (o.ord_no is NULL) or (o.ord_no is not NULL and o.pur_amt > 2000 and c.grade is not NULL);

--16  Write a SQL statement to generate a report with the customer name, city, order no. order date, purchase amount for only those customers on the list who must have a grade and placed one or more orders or which order(s) have been placed by the customer who neither is on the list nor has a grade.
select o.ord_no,o.ord_date,o.pur_amt,c.cust_name,c.city from  customer as c left join orders as o on c.customer_id = o.customer_id where (o.ord_no is NULL and c.grade is NULL) or (o.ord_no is not NULL and c.grade is not NULL);

--17  Write a SQL query to combine each row of the salesman table with each row of the customer table
select * from salesman cross join customer;

--18 Write a SQL statement to create a Cartesian product between salesperson and customer, i.e. each salesperson will appear for all customers and vice versa for that salesperson who belongs to that city
select * from salesman cross join customer;

--19 Write a SQL statement to create a Cartesian product between salesperson and customer, i.e. each salesperson will appear for every customer and vice versa for those salesmen who belong to a city and customers who require a grade
select * from salesman as s cross join customer as c where s.city is not null and c.grade is not null;

--20 Write a SQL statement to make a Cartesian product between salesman and customer i.e. each salesman will appear for all customers and vice versa for those salesmen who must belong to a city which is not the same as his customer and the customers should have their own grade
select * from salesman as s cross join customer as c where s.city != c.city and c.grade is not null;



