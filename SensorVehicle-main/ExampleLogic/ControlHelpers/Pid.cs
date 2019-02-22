using System;

namespace ExampleLogic.L1_CenterCorridor
{
    // Based on https://github.com/ms-iot/pid-controller/blob/master/PidController/PidController/PidController.cs (22.02.2019)
    // TODO: test and correct calculation algorithm. Try to utilize stearing by heading with Pid controller, after we have installed and coded the Gyro.
    public class Pid
    {
        private double _integralTerm;
        private readonly double _minOutputBound;
        private readonly double _maxOutputBound;
        private DateTime _lastInputTime;
        private double _previousProcessValue;

        public Pid(double gainP, double gainI, double gainD, double minOutput, double maxOutput)
        {
            GainD = gainD;
            GainI = gainI;
            GainP = gainP;
            _minOutputBound = minOutput;
            _maxOutputBound = maxOutput;
        }

        public double GainP { get; set; }
        public double GainI { get; set; }
        public double GainD { get; set; }

        private double _processVariable = 0;
        public double ProcessVariable
        {
            get { return _processVariable; }
            set
            {
                PreviousProcessVariable = _processVariable;
                _processVariable = value;
            }
        }

        public double PreviousProcessVariable { get; private set; }

        public double ControlVariable(double setPoint, double processValue)
        {
            TimeSpan timeSinceLastUpdate = DateTime.Now - _lastInputTime;
            _lastInputTime = DateTime.Now;

            double error = setPoint - processValue;

            // P-term calculation
            double proportionalTerm = GainP * error;

            if (timeSinceLastUpdate.Milliseconds > 500) return Clamp(proportionalTerm);

            // I-term calculation
            _integralTerm += Clamp(GainI * error * timeSinceLastUpdate.TotalSeconds);

            // D-term calculation
            double changeSinceLastInput = processValue - _previousProcessValue;
            double derivativeTerm = GainD * (changeSinceLastInput / timeSinceLastUpdate.TotalSeconds);

            _previousProcessValue = processValue;

            // Derivative term is subtracted instead of added, since the derivative is taken of the 'Process Value' instead of the error
            return Clamp(proportionalTerm + _integralTerm - derivativeTerm);
        }

        private double Clamp(double unclampedOutput)
        {
            return unclampedOutput >= _maxOutputBound ? _maxOutputBound : Math.Max(unclampedOutput, _minOutputBound);
        }
    }
}
