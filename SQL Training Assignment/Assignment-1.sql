--Create table dbo.Products(ProductID int primary key Identity(1,1),ProductName varchar(255),SupplierID int,CategoryID int,QuantityPerUnit int,UnitPrice int,UnitsInStock int,UnitsOnOrder int,ReorderLevel int,Discontinued Bit);

-- 1
select ProductId,ProductName, UnitPrice from Products where UnitPrice < 20;

-- 2
select ProductId,ProductName, UnitPrice from Products where UnitPrice between 15 and 25;

--3
select ProductName, UnitPrice from Products where UnitPrice > (SELECT AVG(UnitPrice) FROM Products);

--4
select ProductName, UnitPrice from Products order by UnitPrice desc OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY;

--5
select count(*),case when Discontinued = 1 then 'Discontinue' else 'Current' end as Product_count from Products group by Discontinued;

--6
select ProductName,UnitsOnOrder,UnitsInStock from Products where UnitsInStock<UnitsOnOrder;



select * from Products;

