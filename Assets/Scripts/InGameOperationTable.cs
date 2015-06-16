using UnityEngine;
using System.Collections;

public class InGameOperationTable {

	public InGameOperationTable (int startingFrom,int upToDigitExcluding, bool isDivision=false, int defaultNumberOfRepetitions=3){
		initializeEquationContentsUpTo (startingFrom,upToDigitExcluding,isDivision,defaultNumberOfRepetitions);
	}

	private struct EquationContents{
		public uint remainingRepetitions;
		public uint numberOfCorrectAnswers;
		public uint numberOfWrongAnswers;

		public EquationContents(uint r, uint nc, uint nw){
			remainingRepetitions = r;
			numberOfCorrectAnswers = nc;
			numberOfWrongAnswers = nw;
		}

	}

	private EquationContents[,] equationContents ;

	//Returns how many more equations are left to solve
	public int GetSumOfAllRemainingEquations(){
		uint sum=0;
		for (int i=0; i<11; i++)
			for (int j=0; j<11; j++)
				sum += equationContents [i, j].remainingRepetitions;
		
		return (int) sum;
	}
	//Returns how many more equations are left to solve for a particular digit
	public int GetSumOfAllRemainingEquationsForDigit(int digit){
		uint sum = 0;
		for (int i=0; i<11; i++) {

//				Old way, when table was for multiplication only
//
//			if (i < digit)
//				sum += equationContents [i, digit].remainingRepetitions;
//			else
				sum += equationContents [digit, i].remainingRepetitions;
		}
		return (int)sum;
	}

	public uint GetNumberOfCorrectAnswers(int digit, int digit2){
		return equationContents [digit, digit2].numberOfCorrectAnswers;
	}

	// Return number of days after which equation should be evaluated
	// If there is more than one wrong answer
	public int GetNumberOfDaysUntilRepetition(int i, int j){
		if (equationContents [i, j].numberOfWrongAnswers>= 2) {
			//Should be 1, it is 0 for testing
			return 1;	
		} else if (equationContents [i, j].numberOfWrongAnswers == 1){
			return 2;
		}else if (equationContents [i, j].numberOfWrongAnswers == 0 && equationContents [i, j].remainingRepetitions==0){
			// Two cases - one where there are some correct answers (equations was being tested) 
			// and when there are no answers (disabled equation)
			if(equationContents [i, j].numberOfCorrectAnswers>0){
				return (int)(5+equationContents [i, j].numberOfCorrectAnswers);
			}else{
				return -1;
			}

		}else if (equationContents [i, j].numberOfWrongAnswers == 0 && equationContents [i, j].remainingRepetitions!=0){
			return 0;
		}
		return 1;
	}

	public void resetNumberOfWrongAnswers(){
		for (int i=0; i<11; i++)
						for (int j=0; j<11; j++)
								equationContents [i, j].numberOfWrongAnswers = 0;
	
	}

	// In case of successful answers according measurement properties should be increased.
	public void correctlySolved(int i,int j){
		equationContents [i, j].numberOfCorrectAnswers++;

		//Remaining repetitions should be decreased when particular equation is selected to be solved.
	}


	// In case of mistakes according measurement properties should be increased.
	public void incorrectlySolved(int i,int j){
		equationContents[i, j].numberOfWrongAnswers++;
		equationContents[i, j].remainingRepetitions = 3;
		//Reset number of correct answers in case there were two or more wrong answers
		if (equationContents[i, j].numberOfWrongAnswers>1){
			equationContents[i, j].numberOfCorrectAnswers=0;
		}
	}

	//This is to update number of repetitions with value loaded from file by ProgressManager 
	public void updateNumberOfRepetitionsFor(int i,int j, int howMany=1){
		equationContents [i, j].remainingRepetitions = (uint)Mathf.Max (howMany, equationContents [i, j].remainingRepetitions);
	}

	public uint GetNumberOfRepetitionsFor(int i,int j){
		return equationContents [i, j].remainingRepetitions;
	}

	// Return which digit is best suiting player's learning curve of multiplication table
	// if -1 returned, then probably there is nothing left to calculate (but should not be used)
	public int returnBestDigitForValueOf(int value){
		int returningValue=-1;
		EquationContents eqCont;
		if (GetSumOfAllRemainingEquationsForDigit (value) > 0) {
			do {
				returningValue = Mathf.FloorToInt (Random.Range (0, 11));
				eqCont = equationContents [value, returningValue];
			} while (eqCont.remainingRepetitions<=0);
		}

		/* Old way (fully working, but not randomized
		 * 
		 * 
		int returningValue = -1;
		uint largestRepetitions = 0;

		for (int i = 0 ; i < value;i++)
			if (equationContents[i,value].remainingRepetitions > largestRepetitions){
				largestRepetitions = equationContents[i,value].remainingRepetitions;
				returningValue = i;
			}
		//part 2 from value until 11
		for (int j = value ; j<11;j++)
		if (equationContents[value,j].remainingRepetitions > largestRepetitions){
			largestRepetitions = equationContents[value,j].remainingRepetitions;
			returningValue = j;
		}
*/
		//Decrease number of remaining repetitions of selected equation
		if ((returningValue != -1) && (GetSumOfAllRemainingEquationsForDigit (value) > 0)) {
			equationContents[value,returningValue].remainingRepetitions--;
		}

		return returningValue;
	}

	//Initialize working table of multiplication equations with values of 3,0,0 meaning
	// that every available multiplication should be done correctly 3 times in order to memorize it.
	private void initializeEquationContentsUpTo (int from, int value,bool isDivision,int defaultNumberOfRepetitions){
		if ((0<=from) &&(from<=value) && (value <=11)){
		equationContents = new EquationContents[11,11];

		for (int i=0; i<from; i++) {
				for (int j=0;j<11;j++)
					equationContents[i,j] = new EquationContents(0,0,0);
			}
		for (int i=from; i<value; i++) {
			for (int j=0;j<11;j++)
				equationContents[i,j] = new EquationContents((uint)defaultNumberOfRepetitions,0,0);
		}
		for (int i=value; i<11; i++) {
			for (int j=0;j<11;j++)
				equationContents[i,j] = new EquationContents(0,0,0);
		}

		if (isDivision)
			for (int i=0; i<11; i++)
				equationContents [i, 0] = new EquationContents (0, 0, 0);
		}
	}
}
