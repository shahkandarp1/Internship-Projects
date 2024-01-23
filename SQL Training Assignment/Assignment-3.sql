use kandarpshah_db;--1 write a SQL query to find Employees who have the biggest salary in their Department Find employees who have the biggest salary in their department
SELECT e.emp_id, e.emp_name, e.salary, d.dept_name FROM employee e JOIN department d ON e.dept_id = d.dept_id WHERE e.salary = (  SELECT MAX (salary)  FROM employee  WHERE dept_id = e.dept_id );

--2 write a SQL query to find Departments that have less than 3 people in it
select d.dept_name from department as d inner join employee as e on d.dept_id = e.dept_id group by d.dept_name having count(e.emp_id) < 3

--3 write a SQL query to find All Department along with the number of people there
select d.dept_name,count(e.emp_id) from department as d left join employee as e on d.dept_id = e.dept_id group by d.dept_name

--4 write a SQL query to find All Department along with the total salary there
select d.dept_name,sum(e.salary) from department as d left join employee as e on d.dept_id = e.dept_id group by d.dept_name
