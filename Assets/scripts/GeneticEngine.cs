using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// A singleton engine for performing the genetic algorithm's optimization strategy
// to find the optimal solution for maintaining a plant to get maximum growth
public class GeneticEngine : MonoBehaviour {

	// Process:
	// 		1. Generate random population
	// 		2. Evaulate the solutions
	// 		3. Select parents
	// 		4. Create child solutions
	// 		5. Evaluate children
	// 		6. Swap some existing solutions with better ones
	// 		Loop steps 2-6 until termination criteria is met

	private List<Solution> population;
	public uint POPULATION_SIZE = 10;
	public uint PARENT_COUNT = 5;
	public uint MUTATION_ITERATIONS = 1;
	public float MUTATION_CHANCE = 0.1f;
	public float fitnessGoal = 100.0f;
	public ulong iterations = 1000;

	public float displayScale = 0.01f;

	public Text displayText;
	public Transform bestBush;

	private GeneticEngine instance;
	private Stack<int> parentIndices;
	private ulong currentIteration = 0;
	private Solution best;
	private bool foundBest = false;

	public GeneticEngine getInstance() {
		if (instance == null) {
			instance = new GeneticEngine ();
		}

		return instance;
	}

	// Use this for initialization
	void Awake() {
		Debug.Log ("Initializing GeneticEngine...");
		population = new List<Solution> ();
		parentIndices = new Stack<int> ();
		GeneratePopulation ();
	}

	// Update is called once per frame
	public void Update() {
		if (currentIteration < iterations) {
			best = population [getBestIndex ()];
//			Debug.Log ("Current best: " + best.toString ());
			EvaluatePopulation ();
			ChooseParents ();
			Repopulate ();
			best = population [getBestIndex ()];

			if (currentIteration % 10 == 0) {
				displayText.text = best.toString ();
				float radius = best.plant.getRadius ();
				Debug.Log ("Iteration " + currentIteration + "/" + iterations + " (Radius: " + radius + ")");
				bestBush.localScale = new Vector3 (radius * displayScale, best.plant.getHeight() * displayScale, radius * displayScale);
			}

			currentIteration++;
		} else if (!foundBest) {
//			bestBush.localScale = new Vector3 (best.plant.bushRadius * displayScale, best.plant.bushHeight * displayScale, best.plant.bushRadius * displayScale);
			Debug.Log ("Found BEST solution " + best.toString ());
			foundBest = true;
			displayText.text = best.toString ();
		}
	}

	void GeneratePopulation() 
	{
		population.Clear ();

		for (int i = 0; i < POPULATION_SIZE; i++) 
		{
			population.Add (Solution.getRandomSolution ());
		}
	}

	void EvaluatePopulation()
	{
		foreach(Solution solution in population)
		{
			solution.Evaluate ();
		}
	}

	void ChooseParents()
	{
		if (parentIndices == null) {
			parentIndices = new Stack<int> ();
		}
		parentIndices.Clear ();

		for (int i = 0; i < PARENT_COUNT; i++) {
			int index = (int) Random.Range (0.0f, PARENT_COUNT);
			if (!parentIndices.Contains(index))
				parentIndices.Push (index);
		}
	}

	void Repopulate()
	{
		while (parentIndices.Count > 1) {
			int parentAIndex = parentIndices.Pop ();
			int parentBIndex = parentIndices.Pop ();

			Solution parentA = population [parentAIndex];
			Solution parentB = population [parentBIndex];
			Solution child = parentA.Crossover (parentB);

			if (Random.Range (0.0f, 1.0f) < MUTATION_CHANCE) {
				string original = child.toString ();
				child = child.Mutated (MUTATION_ITERATIONS);
				Debug.Log ("Mutated child " + original + " to " + child.toString ());
			}

			child.Evaluate ();
				
			int worstIndex = getWorstIndex ();
			Solution worst = population [worstIndex];

			if (child.fitness > worst.fitness) {
				Debug.Log ("Swapping child " + worst.toString () + " for better " + child.toString ());
				population [worstIndex] = child;
			}
		}
	}

	private int getWorstIndex()
	{
		Solution worst = population [0];
		int index = 0;
		for (int i = 0; i < population.Count; i++) {
			Solution solution = population [i];
			if (solution.fitness <= worst.fitness) {
				worst = solution;
				index = i;
			}
		}

		return index;
	}

	private int getBestIndex()
	{
		Solution best = population [0];
		int index = 0;
		for (int i = 0; i < population.Count; i++) {
			Solution solution = population [i];
			if (solution.fitness >= best.fitness) {
				best = solution;
				index = i;
			}
		}

		return index;
	}
}
