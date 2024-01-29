-- Folder 3
select distinct customer_id from payment where amount >= 7.99;
select title,rental_rate,replacement_cost from film where rental_rate > 2.99 or replacement_cost > 19.99;

-- Folder 4
select title,release_year,rental_duration,replacement_cost from film where rental_duration between 4 and 6 order by replacement_cost desc limit 100; 
select * from film where length > 120 and rating in ('G','PG') and description like '%Action%';

-- Folder 6
select first_name , count(*) from actor group by first_name order by count(*) desc;

--Folder 8
select l.name ,f.title,f.rental_rate from film as f inner join public.language as l on f.language_id = l.language_id; 
select first_name,last_name,count(*) from film_actor as fa inner join actor as ac on ac.actor_id = fa.actor_id group by first_name,last_name order by count(*) desc
select rating,count(*) from film as f inner join inventory as i on i.film_id = f.film_id inner join rental as r on i.inventory_id = r.inventory_id group by rating order by count(*) desc 

--Folder 10
select rental_date,return_date,age(return_date,rental_date) as rent_duration, first_name,last_name,email from rental as r inner join customer as c on r.customer_id = c.customer_id where extract(day from age(return_date,rental_date))>=7 and return_date is not null order by rent_duration desc;
select title,length(title),substr(title,10),substr(title,15),substr(title,5,3),substr(title,5,1) from film where length(title) >= 15;

--Folder 12
select concat(first_name,' ',last_name),email,sum(amount),case when sum(amount)>=200 then 'Elite' when sum(amount)<200 and sum(amount)>=150 then 'Platinum' when sum(amount)<150 and sum(amount)>=100 then 'Gold' else 'Silver' end as cutomer_category from payment inner join customer on payment.customer_id = customer.customer_id group by first_name,last_name,email;
Create view get_total_rental as select concat(first_name,' ',last_name),email,sum(amount),case when sum(amount)>=200 then 'Elite' when sum(amount)<200 and sum(amount)>=150 then 'Platinum' when sum(amount)<150 and sum(amount)>=100 then 'Gold' else 'Silver' end as cutomer_category from payment inner join customer on payment.customer_id = customer.customer_id group by first_name,last_name,email;

--Folder 14
CREATE TABLE order_details ( orderid INTEGER PRIMARY KEY, customer_name VARCHAR (50) NOT NULL, product_name VARCHAR (50) NOT NULL, ordered_from VARCHAR (50) NOT NULL, order_amount NUMERIC (7,2), order_date DATE NOT NULL, delivery_date DATE)
ALTER TABLE order_details RENAME COLUMN customer_name TO customer_first_name;
ALTER TABLE order_details ADD COLUMN cancel_date DATE;