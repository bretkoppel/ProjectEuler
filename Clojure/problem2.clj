(ns clojure-problem2
	(:require [clojure.test :as test]))

(defn fib-even [limit]
  (loop [x 0 y 1 acc 0]
   (let [term (+ x y)]
    (if (< y limit)
     (recur y term (if (even? term) (+ acc term) acc))
     acc))))
(println (fib-even 4000000))
(test/is (= (fib-even 4000000) 4613732) "Solution 1")