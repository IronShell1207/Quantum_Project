/*
 * Quantum.NET
 * A library to manipulate qubits and simulate quantum circuits
 * Author: Pierre-Henry Baudin
 */

using System;
using System.Numerics;

namespace Lachesis.QuantumComputing
{
	public class Qubit : QuantumRegister
	{
        /*
		 * Амплитуда вероятности для состояния |0> Probability amplitude for state |0>
		 */
        public Complex ZeroAmplitude
		{
			get
			{
				return this.Vector.At(0);
			}
			private set
			{
				this.Vector.At(0, value);
			}
		}

        /*
		 * Амплитуда вероятности для состояния |1> Probability amplitude for state |1>
		 */
        public Complex OneAmplitude
		{
			get
			{
				return this.Vector.At(1);
			}
			private set
			{
				this.Vector.At(1, value);
			}
		}

        /*
		 * Конструктор из амплитуд вероятности Constructor from probability amplitudes
		 */
        public Qubit(Complex zeroAmplitude, Complex oneAmplitude) : base(zeroAmplitude, oneAmplitude) { }

        /*
		 * Конструктор из частей амплитуд вероятности Constructor from parts of probability amplitudes
		 */
        public Qubit(double zeroAmplitudeReal, double zeroAmplitudeImaginary, double oneAmplitudeReal, double oneAmplitudeImaginary) : base(new Complex(zeroAmplitudeReal, zeroAmplitudeImaginary), new Complex(oneAmplitudeReal, oneAmplitudeImaginary)) { }

        /*
		 * Конструктор из блоховских сферных координат Constructor from Bloch sphere coordinates
		 */
        public Qubit(double colatitude, double longitude) : base(Math.Cos(colatitude / 2), Math.Sin(colatitude / 2) * Mathematics.Numerics.ComplexExp(Complex.ImaginaryOne * longitude)) { }

        /*
		 * Нормализует кубит Normalizes a qubit
		 */
        protected override void Normalize()
		{
            // Нормализовать величину Normalize magnitude
            base.Normalize();

            // Нормализовать фазу Normalize phase
            if (this.ZeroAmplitude.Phase != 0)
			{
				this.ZeroAmplitude = this.ZeroAmplitude * Complex.FromPolarCoordinates(1, -this.ZeroAmplitude.Phase);
				this.OneAmplitude = this.OneAmplitude * Complex.FromPolarCoordinates(1, -this.ZeroAmplitude.Phase);
			}
		}

		/*
		 * |0>
		 */
		public static Qubit Zero
		{
			get
			{
				return new Qubit(Complex.One, Complex.Zero);
			}
		}

		/*
		 * |1>
		 */
		public static Qubit One
		{
			get
			{
				return new Qubit(Complex.Zero, Complex.One);
			}
		}
	}
}
