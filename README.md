MapShower.cs:

in Start method
1. Calculate remap tex (current get remap tex from file)
2. Create Pallete tex & fill white collor
3. Create Pallete offsets list

in Update method
1. Check buttons
2. Check mouse position & color pallete by pos


BorderShower.cs:

in Start method
1. Calc border points CalculatePoints(mainArr);
2. Clear doubles from point list ClearPointsIntList(_bordersInt);
3. Convert int point to float
4. Add borders to list that save border by border color
5. Check that bordes do not heve border with one point
6. Sorting point into line
7. Additional filtering???
8. Calc mesh size

in Update method
1. increment/decrement province border enumerator(used for select part of province boder)
2. Calc mouse pos and drawing border if clicked
