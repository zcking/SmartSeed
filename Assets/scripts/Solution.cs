using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// Defines a "solution" which is a configuration for growing a plant
// This acts as the chromosome in this Genetic Algorithm approach
[System.Serializable]
public class Solution
{
	[Range(0.0f, 1.0f)] public float soilMoisture = 0.0f; 	// 0-1 Normalized scale
	[Range(0.0f, 1.0f)] public float sunlight = 0.0f; 		// 0-1 Normalized scale
	[Range(0.0f, 1.0f)] public float humidity = 0.0f; 		// 0-1 Normalized scale
	[Range(0.0f, 100.0f)] public float temperature = 0.0f; 	// Degrees Fahrenheit

	public Plant plant;
	public double fitness = 0.0f;

	public Solution()
	{
		this.plant = new Plant ();
		plant.Init (this);
	}

	public Solution(Solution other) {
		this.plant = other.plant;
		this.soilMoisture = other.soilMoisture;
		this.sunlight = other.sunlight;
		this.humidity = other.humidity;
		this.temperature = other.temperature;

		this.fitness = other.fitness;
	}

	public static Solution getRandomSolution() 
	{
		Solution sol = new Solution ();
		sol.soilMoisture = Random.Range (0.5f, 0.8f);
		sol.sunlight = Random.Range (0.65f, 0.71f);
		sol.humidity = Random.Range (0.8f, 0.9f);
		sol.temperature = Random.Range (60.0f, 80.0f);
		return sol;
	}

	public void Evaluate()
	{
		// Evaluate the solution based on the plant
		fitness = plant.CalculateFitness() * 10000.0f;
	}

	public Solution Mutated(uint iterations)
	{
		Solution sol = new Solution (this);

		for (int i = 0; i < iterations; i++) {
			float n = Random.Range (0.0f, 4.0f);
			if (n < 1.0f)
				sol.soilMoisture = Random.Range (0.70f, 0.8f);
			else if (n < 2.0f)
				sol.sunlight = Random.Range (0.65f, 0.70f);
			else if (n < 3.0f)
				sol.humidity = Random.Range (0.8f, 0.9f);
			else
				sol.temperature = Random.Range (60.0f, 80.0f);
		}

		sol.Evaluate ();

		return sol;
	}

	public Solution Crossover(Solution other)
	{
		Solution sol = new Solution ();
		sol.soilMoisture = crossoverChooser (this.soilMoisture, other.soilMoisture);
		sol.sunlight = crossoverChooser (this.sunlight, other.sunlight);
		sol.humidity = crossoverChooser (this.humidity, other.humidity);
		sol.temperature = crossoverChooser (this.temperature, other.temperature);
		return sol;
	}

	private float crossoverChooser(float self, float other)
	{
		if (Random.Range(0.0f, 1.0f) > 0.5f)
			return other;
		else
			return self;
	}

	public string toString() {
		return "Solution(SM: " + soilMoisture + ", SL: " + sunlight + ", TP: " + temperature + ", HM: " + humidity + ") -> " + fitness;
	}

}

