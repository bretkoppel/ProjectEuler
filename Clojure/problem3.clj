(ns clojure-problem3
	(:require [clojure.test :as test]))

(defn prime-factors-by-trial-division
	"Gets the frequencies of each prime factor for value."
	[value]	
	(loop [v value testers (range 3 value 2) primes []]
		(let [t (first testers)]
			(cond
				(< v 3) (frequencies (conj primes v))
				(.isProbablePrime (BigInteger. (str v)) 3) (frequencies (conj primes v))
				(zero? (mod v 2)) (recur (/ v 2) testers (conj primes 2))
				(and (.isProbablePrime (BigInteger. (str t)) 3) (zero? (mod v t))) (recur (/ v t) testers (conj primes t))
				:else (recur v (next testers) primes)
			)
		)
	)
)
(println "Factor Frequencies: " (prime-factors-by-trial-division 600851475143))
(println "Max Factor: " (first (sort-by - (keys (prime-factors-by-trial-division 600851475143)))))
(test/is (=  (first (sort-by - (keys (prime-factors-by-trial-division 600851475143)))) 6857) "Solution 1")