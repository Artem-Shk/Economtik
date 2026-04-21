CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Справочник методов
CREATE TABLE methods (
                         id SERIAL PRIMARY KEY,
                         code VARCHAR(50) UNIQUE NOT NULL,
                         name VARCHAR(100) NOT NULL
);

INSERT INTO methods (code, name) VALUES
                                     ('simpson', 'Метод Симпсона'),
                                     ('trapezoidal', 'Метод трапеций'),
                                     ('monte_carlo', 'Метод Монте-Карло');

-- История вычислений
CREATE TABLE computations (
                              id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
                              function_expr VARCHAR(500) NOT NULL,
                              lower_bound DECIMAL(18,6) NOT NULL,
                              upper_bound DECIMAL(18,6) NOT NULL,
                              method_id INT REFERENCES methods(id),
                              steps INT NOT NULL,
                              result DECIMAL(18,10) NOT NULL,
                              duration_ms INT NOT NULL,
                              created_at TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_computations_created ON computations(created_at DESC);