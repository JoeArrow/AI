using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace LinRegOnnxRunner
{
    public partial class AIPredForm : Form
    {
        private readonly InferenceSession _session;
        private readonly string _inputName;

        public AIPredForm()
        {
            InitializeComponent();

            var modelPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "linreg_valid_v1.onnx"
            );

            _session = new InferenceSession(modelPath);
            _inputName = _session.InputMetadata.Keys.Single();
        }

        // ------------------------------------------------

        private void btnPredict_Click(object sender, EventArgs e)
        {
            float x = (float)numX.Value;

            var input = new DenseTensor<float>(new[] { 1, 1 });
            input[0, 0] = x;

            var inputs = new[]
            {
                NamedOnnxValue.CreateFromTensor(_inputName, input)
            };

            using var results = _session.Run(inputs);

            float y = results
                .First()
                .AsTensor<float>()[0, 0];

            txtY.Text = y.ToString("0");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _session.Dispose();
            base.OnFormClosed(e);
        }
    }
}

