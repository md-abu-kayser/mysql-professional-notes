-- Top N products by sales
SELECT p.name, SUM(oi.quantity) AS total_sold
FROM products p JOIN order_items oi ON p.id = oi.product_id
GROUP BY p.id ORDER BY total_sold DESC LIMIT 10;

-- Users with no orders
SELECT * FROM users u WHERE NOT EXISTS (SELECT 1 FROM orders o WHERE o.user_id = u.id);

-- Update with join
UPDATE users u JOIN orders o ON u.id = o.user_id SET u.last_order_date = o.created_at;