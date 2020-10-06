/*
 * Quantum.NET
 * A library to manipulate qubits and simulate quantum circuits
 * Author: Pierre-Henry Baudin
 */

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Lachesis.QuantumComputing
{
	public class QuantumGate
	{
        /*
		 * Матричное представление квантовых вентилей 
         * Matrix representation of a quantum gate
		 */
        public Matrix<Complex> Matrix { get; private set; }

        /*
		 * Конструктор из других квантовых вентилей 
         * Constructor from other quantum gates
		 */
        public QuantumGate(params QuantumGate[] quantumGates) : this((IEnumerable<QuantumGate>)quantumGates) { }
        /*
		 * Конструктор из множества других квантовых вентилей 
         * Constructor from enumerable of other quantum gates
		 */
        public QuantumGate(IEnumerable<QuantumGate> quantumGates)
		{
			this.Matrix = quantumGates.Aggregate(Matrix<Complex>.Build.Sparse(1, 1, Complex.One), (matrix, quantumGate) => matrix.KroneckerProduct(quantumGate.Matrix));
		}

        /*
		 * Конструктор из двумерного массива комплексных коэффициентов
         * Constructor from a bidimensional array of complex coefficients
		 */
        public QuantumGate(Complex[,] coefficients) : this(Matrix<Complex>.Build.SparseOfArray(coefficients)) { }

        /*
		 * Конструктор из матричного представления
         * Constructor from matrix representation
		 */
        public QuantumGate(Matrix<Complex> matrix)
		{
			if ((matrix.ColumnCount & (matrix.ColumnCount - 1)) != 0 || matrix.ColumnCount != matrix.RowCount)
			{
				throw new ArgumentException("A quantum gate can only be initialized from a square matrix whose order is a power of 2.");
			}

			this.Matrix = matrix;
		}

        /*
		 * Идентификационные вентили
         * Identity gate
		 */
        public static QuantumGate IdentityGate
		{
			get
			{
				return QuantumGate.IdentityGateOfLength(1);
			}
		}

        /*
		 * Сложенные идентификационные вентили
         * Stacked identity gates
		 */
        public static QuantumGate IdentityGateOfLength(int registerLength)
		{
			return new QuantumGate(Matrix<Complex>.Build.SparseIdentity(1 << registerLength));
		}

        /*
         * Вентиль Адамара
		 * Hadamard gate
		 */
        public static QuantumGate HadamardGate
		{
			get
			{
				return new QuantumGate(Matrix<Complex>.Build.SparseOfArray(new Complex[,] {
					{ 1, 1 },
					{ 1, -1 },
				}) / Math.Sqrt(2));
			}
		}

        /*
		 * Обобщенный Вентиль Адамара
         * Generalized Hadamard gate
		 */
        public static QuantumGate HadamardGateOfLength(int registerLength)
		{
			if (registerLength == 1)
			{
				return QuantumGate.HadamardGate;
			}
			else
			{
				return new QuantumGate(QuantumGate.HadamardGate, QuantumGate.HadamardGateOfLength(registerLength - 1));
			}
		}
        /*
		 * NOT gate (Pauli-X вентиль)
		 */
        public static QuantumGate NotGate
		{
			get
			{
				return new QuantumGate(new Complex[,] {
					{ 0, 1 },
					{ 1, 0 },
				});
			}
		}

		/*
		 * Pauli-Y gate (вентиль)
		 */
		public static QuantumGate PauliYGate
		{
			get
			{
				return new QuantumGate(new Complex[,] {
					{ 0, -Complex.ImaginaryOne },
					{ Complex.ImaginaryOne, 0 },
				});
			}
		}

		/*
		 * Pauli-Z gate (вентиль)
		 */
		public static QuantumGate PauliZGate
		{
			get
			{
				return new QuantumGate(new Complex[,] {
					{ 1, 0 },
					{ 0, -1 },
				});
			}
		}
        public static QuantumGate gate1
        {
            get
            {
                return new QuantumGate(new Complex[,] {
                    { 1, 0 },
                    { 0, 1 },
                });
            }
        }
        /*
		 * Квадратный корень вентиля НЕ
         * Square root of NOT gate
		 */
        public static QuantumGate SquareRootNotGate
		{
			get
			{
				return new QuantumGate(Matrix<Complex>.Build.SparseOfArray(new Complex[,] {
					{ 1 + Complex.ImaginaryOne, 1 - Complex.ImaginaryOne },
					{ 1 - Complex.ImaginaryOne, 1 + Complex.ImaginaryOne },
				}) / 2);
			}
		}

        /*
         * Фаза сдвига вент
		 * Phase shift gate
		 */
        public static QuantumGate PhaseShiftGate(double phase)
		{
			return new QuantumGate(new Complex[,] {
				{ 1, 0 },
				{ 0, Mathematics.Numerics.ComplexExp(Complex.ImaginaryOne * phase)},
			});
		}
        public static QuantumGate PhaseGate
        {
            get
            {
                return new QuantumGate(new Complex[,] {
                { 1, 0 },
                { 0, Complex.ImaginaryOne},
            });
            }
        }
        /*
         * Сменные ворота
		 * Swap gate
		 */
        public static QuantumGate SwapGate
		{
			get
			{
				return new QuantumGate(new Complex[,] {
					{ 1, 0, 0, 0 },
					{ 0, 0, 1, 0 },
					{ 0, 1, 0, 0 },
					{ 0, 0, 0, 1 },
				});
			}
		}
        // вентиль оракула
        public static QuantumGate OracleGate
        {
            get
            {
                return new QuantumGate(new Complex[,] {
                    { 0, 1, 0, 0 },
                    { 1, 0, 0, 0 },
                    { 0, 0, 0, 1 },
                    { 0, 0, 1, 0 },
                });
            }
        }
        /*
         * Квадратный корень из своп вентиля
		 * Square root of swap gate
		 */
        public static QuantumGate SquareRootSwapGate
		{
			get
			{
				return new QuantumGate(new Complex[,] {
					{ 1, 0, 0, 0 },
					{ 0, (1 + Complex.ImaginaryOne) / 2, (1 - Complex.ImaginaryOne) / 2, 0 },
					{ 0, (1 - Complex.ImaginaryOne) / 2, (1 + Complex.ImaginaryOne) / 2, 0 },
					{ 0, 0, 0, 1 },
				});
			}
		}

        /*
         * Контролируемый вентиль НЕ
		 * Controlled NOT gate
		 */
        public static QuantumGate ControlledNotGate
		{
			get
			{
				return QuantumGate.ControlledGate(QuantumGate.NotGate);
			}
		}

		/*
		 * Контролируемый вентиль
         * Controlled gate
		 */
		public static QuantumGate ControlledGate(QuantumGate gate)
		{
			if (gate.Matrix.ColumnCount != 2 || gate.Matrix.RowCount != 2)
			{
				throw new ArgumentException("A controlled gate can only be created from a unary gate.");
			}

			return new QuantumGate(new Complex[,] {
				{ 1, 0, 0, 0 },
				{ 0, 1, 0, 0 },
				{ 0, 0, gate.Matrix.At(0, 0), gate.Matrix.At(0, 1) },
				{ 0, 0, gate.Matrix.At(1, 0), gate.Matrix.At(1, 1) },
			});
		}

		/*
         * Вентиль Тоффоли
		 * Toffoli gate
		 */
		public static QuantumGate ToffoliGate
		{
			get
			{
				return new QuantumGate(new Complex[,] {
					{ 1, 0, 0, 0, 0, 0, 0, 0 },
					{ 0, 1, 0, 0, 0, 0, 0, 0 },
					{ 0, 0, 1, 0, 0, 0, 0, 0 },
					{ 0, 0, 0, 1, 0, 0, 0, 0 },
					{ 0, 0, 0, 0, 1, 0, 0, 0 },
					{ 0, 0, 0, 0, 0, 1, 0, 0 },
					{ 0, 0, 0, 0, 0, 0, 0, 1 },
					{ 0, 0, 0, 0, 0, 0, 1, 0 },
				});
			}
		}

        /*
         * Вентиль Фредкинский
		 * Fredkin gate
		 */
        public static QuantumGate FredkinGate
		{
			get
			{
				return new QuantumGate(new Complex[,] {
					{ 1, 0, 0, 0, 0, 0, 0, 0 },
					{ 0, 1, 0, 0, 0, 0, 0, 0 },
					{ 0, 0, 1, 0, 0, 0, 0, 0 },
					{ 0, 0, 0, 1, 0, 0, 0, 0 },
					{ 0, 0, 0, 0, 1, 0, 0, 0 },
					{ 0, 0, 0, 0, 0, 0, 1, 0 },
					{ 0, 0, 0, 0, 0, 1, 0, 0 },
					{ 0, 0, 0, 0, 0, 0, 0, 1 },
				});
			}
		}

        /*
         * Квантовое преобразование Фурье
		 * Quantum Fourier transform
		 */
        public static QuantumGate QuantumFourierTransform(int registerLength)
		{
			int order = 1 << registerLength;

			Matrix<Complex> matrix = Matrix<Complex>.Build.Sparse(order, order);

            // В матрице квантового преобразования Фурье найдено только n различных коэффициентов
            // Only n distinct coefficients are found in the quantum Fourier transform matrix
            Complex[] coefficients = new Complex[order];
			for (int i = 0; i < order; i++)
			{
				coefficients[i] = Mathematics.Numerics.ComplexExp(Complex.ImaginaryOne * 2 * Math.PI * i / order) / Math.Sqrt(order);
			}

            // Заполнить матрицу  Populate matrix
            for (int i = 0; i < order; i++)
			{
				for (int j = 0; j < order; j++)
				{
					matrix.At(i, j, coefficients[i * j % order]);
				}
			}

			return new QuantumGate(matrix);
		}

        /*
         * Оператор для применения квантового вентиля к квантовому регистру
		 * Operator to apply a quantum gate to a quantum register
		 */
        public static QuantumRegister operator *(QuantumGate quantumGate, QuantumRegister quantumRegister)
		{
			return new QuantumRegister(quantumGate.Matrix * quantumRegister.Vector);
		}

        /*
         * Строковое представление квантовых вентилей
		 * String representation of a quantum gate
		 */
        public override string ToString()
		{
			return this.Matrix.ToString();
		}

        /*
         * Определяет, равен ли указанный квантовый вентиль текущему квантовому вентилю
		 * Determines whether the specified quantum gate is equal to the current quantum gate
		 */
        public override bool Equals(object obj)
		{
			QuantumGate quantumGate = obj as QuantumGate;

			if (quantumGate == null || this.Matrix.ColumnCount != quantumGate.Matrix.ColumnCount || this.Matrix.RowCount != quantumGate.Matrix.RowCount)
			{
				return false;
			}

			return this.Matrix.Equals(quantumGate.Matrix);
		}

        /*
         * Определяет, равен ли указанный квантовый вентиль текущему квантовому вентилю, игнорируя проблемы точности с плавающей точкой
		 * Determines whether the specified quantum gate is equal to the current quantum gate, ignoring floating-point precision issues
		 */
        public bool AlmostEquals(object obj)
		{
			QuantumGate quantumGate = obj as QuantumGate;

			if (quantumGate == null || this.Matrix.ColumnCount != quantumGate.Matrix.ColumnCount || this.Matrix.RowCount != quantumGate.Matrix.RowCount)
			{
				return false;
			}

			return Precision.AlmostEqual<Complex>(this.Matrix, quantumGate.Matrix, 15);
		}

        /*
         * Служит хеш-функцией для квантовых вентилей
		 * Serves as a hash function for a quantum gate
		 */
        public override int GetHashCode()
		{
			return this.Matrix.GetHashCode();
		}
	}
}
