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

