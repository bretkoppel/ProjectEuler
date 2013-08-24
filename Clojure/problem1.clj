(ns clojure-problem1
	(:require [clojure.test :as test]))

;sln 1
;very slow
(defn solution-1 []
	(reduce +
	  (filter 
	    #(or (= 0 (mod % 3)) (= 0 (mod % 5)))
	    (range 3 1000)))
	)
(println (solution-1))
(test/is (= (solution-1) 233168) "Solution 1")

;sln 2
;much faster(~75%)
(defn solution-2 []
	(reduce +
		(reduce + (range 3 1000 3))
		(filter #(not (zero? (mod % 3))) (range 5 1000 5))))
(println (solution-1))
(test/is (= (solution-2) 233168) "Solution 2")


;sln 3
;slightly faster(~10%)
(defn threes-and-fives [limit]
  (letfn [(multiplier [n acc]
    (let [three (* 3 n) five (* 5 n)]
      (if (>= three limit)
        acc
        (recur (inc n)
        	(if (or (>= five limit) (zero? (mod five 3)))
        		(+ three acc)
        		(+ three five acc)
        	))
      )))]
    (multiplier 1 0)))
(println (threes-and-fives 1000))
(test/is (= (threes-and-fives 1000) 233168) "Solution 3")