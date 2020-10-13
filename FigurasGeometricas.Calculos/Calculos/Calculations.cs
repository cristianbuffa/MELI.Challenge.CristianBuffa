using System;
using System.Collections.Generic;
using System.Linq;
using FigurasGeometricas.Calculos.Entidades;
using MathNet.Symbolics;
using Expr = MathNet.Symbolics.Expression;

namespace GeometricsShapes.Calculations
{
    public interface ICalculations
    {
        public Coordinates GetXYbyCircunsferencesIntersection(Circumference circumference1, Circumference
           circumference2, Circumference circumference3);
    }

    public class Calculations : ICalculations
    {
        public Coordinates GetXYbyCircunsferencesIntersection(Circumference circumference1, Circumference
           circumference2, Circumference circumference3)
        {
            var response = new Coordinates() { X = 0, Y = 0 };
            var circumferenceVariables = new List<Variables>()
            { new Variables() { 
                                Name = "CircumferenceEquation"    
                                ,V_2_X_CoordX = ((-1)*circumference1.Coordinates.X) * 2
                                ,L_CoordX_CoordX = circumference1.Coordinates.X * circumference1.Coordinates.X
                                ,V_2_Y_CoordY = ((-1)*circumference1.Coordinates.Y) * 2
                                ,L_CoordY_CoordY = circumference1.Coordinates.Y * circumference1.Coordinates.Y
                                ,Radio = circumference1.Radio * circumference1.Radio
                                ,ReductionFactor = 1
                },
            new Variables() {   Name = "CircumferenceEquation"
                                ,V_2_X_CoordX = ((-1)*circumference2.Coordinates.X) * 2
                                ,L_CoordX_CoordX = (circumference2.Coordinates.X * circumference2.Coordinates.X)
                                ,V_2_Y_CoordY = ((-1)*circumference2.Coordinates.Y) * 2
                                ,L_CoordY_CoordY = (circumference2.Coordinates.Y * circumference2.Coordinates.Y)
                                ,Radio = (circumference2.Radio * circumference2.Radio)
                                ,ReductionFactor = -1

            } };

            //Get First Equation to substitution on the last step

            var equationForFinalSubstitution = string.Format("x^2+y^2{0}*x{1}{2}*y{3}{4}", (circumferenceVariables[0].V_2_X_CoordX  > 0 ? "+" : "") + circumferenceVariables[0].V_2_X_CoordX
                                                                , (circumferenceVariables[0].L_CoordX_CoordX > 0 ? "+" : "") + circumferenceVariables[0].L_CoordX_CoordX
                                                                , (circumferenceVariables[0].V_2_Y_CoordY > 0 ? "+" : "") + circumferenceVariables[0].V_2_Y_CoordY
                                                                , (circumferenceVariables[0].L_CoordY_CoordY > 0 ? "+" : "") + circumferenceVariables[0].L_CoordY_CoordY
                                                                , ((-1)*circumferenceVariables[0].Radio) > 0 ? "+" : "") + ((-1)*circumferenceVariables[0].Radio);

            //Apply factor for reduction
            circumferenceVariables[1].V_2_X_CoordX *= circumferenceVariables[1].ReductionFactor;
            circumferenceVariables[1].L_CoordX_CoordX *= circumferenceVariables[1].ReductionFactor;
            circumferenceVariables[1].V_2_Y_CoordY *= circumferenceVariables[1].ReductionFactor;
            circumferenceVariables[1].L_CoordY_CoordY *= circumferenceVariables[1].ReductionFactor;
            circumferenceVariables[1].Radio *= circumferenceVariables[1].ReductionFactor;
          

            //Apply Reduction

            var reducedEquationSum =
            from item in circumferenceVariables
            group item by item.Name into reducedEquationGroup
            select new
            {
                Name = reducedEquationGroup.Key,
                V_2_X_CoordX = reducedEquationGroup.Sum(x => x.V_2_X_CoordX),
                L_CoordX_CoordX = reducedEquationGroup.Sum(x => x.L_CoordX_CoordX),
                V_2_Y_CoordY = reducedEquationGroup.Sum(x => x.V_2_Y_CoordY),
                L_CoordY_CoordY = reducedEquationGroup.Sum(x => x.L_CoordY_CoordY),
                Radio = reducedEquationGroup.Sum(x => x.Radio)
            };

            var reducedEquation  = reducedEquationSum.ToList();
            var GCDInValues = new int[3] { Math.Abs((int)reducedEquation[0].V_2_X_CoordX),
                                           Math.Abs((int)reducedEquation[0].V_2_Y_CoordY),
                                           Math.Abs((int)reducedEquation[0].L_CoordX_CoordX + (int)reducedEquation[0].L_CoordY_CoordY) };

            string generalExpression = string.Format("{0}*x{1}{2}*y{3}{4}", reducedEquation[0].V_2_X_CoordX 
                                                                ,(reducedEquation[0].L_CoordX_CoordX > 0 ? "+": "") + reducedEquation[0].L_CoordX_CoordX
                                                                ,(reducedEquation[0].V_2_Y_CoordY > 0 ? "+" : "") + reducedEquation[0].V_2_Y_CoordY
                                                                ,(reducedEquation[0].L_CoordY_CoordY > 0 ? "+" : "") +  reducedEquation[0].L_CoordY_CoordY
                                                                ,(((-1)*reducedEquation[0].Radio) > 0 ? "+" : "") + (-1)*reducedEquation[0].Radio);

            var x = Expr.Symbol("x");
            var y = Expr.Symbol("y");

            Expr aleft = Infix.ParseOrThrow(generalExpression);
            Expr aright = Infix.ParseOrThrow("0");
            //Resolve "x" from general expression
            Expr xFromGeneralExpression = SolveSimpleRoot(x, aleft - aright); 

            //TODO Substitution and final result
          
            return response;
        }

        Expr SolveSimpleRoot(Expr variable, Expr expr)
        {
            // try to bring expression into polynomial form
            Expr simple = Algebraic.Expand(Rational.Numerator(Rational.Simplify(variable, expr)));

            // extract coefficients, solve known forms of order up to 1
            Expr[] coeff = Polynomial.Coefficients(variable, simple);
            switch (coeff.Length)
            {
                case 1: return Expr.Zero.Equals(coeff[0]) ? variable : Expr.Undefined;
                case 2: return Rational.Simplify(variable, Algebraic.Expand(-coeff[0] / coeff[1]));
                default: return Expr.Undefined;
            }
        }

    }

}
