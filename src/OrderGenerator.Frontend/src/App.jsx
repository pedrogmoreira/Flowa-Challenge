import { useState } from 'react'
import OrderForm from './components/OrderForm'
import OrderResult from './components/OrderResult'

function App() {
  const [result, setResult] = useState(null)
  const [error, setError] = useState(null)
  const [loading, setLoading] = useState(false)

  return (
    <div className="container">
      <h1>Order Generator</h1>
      <OrderForm
        setResult={setResult}
        setError={setError}
        setLoading={setLoading}
        loading={loading}
      />
      {loading && <p className="loading">Sending order...</p>}
      {error && <p className="error">{error}</p>}
      {result && <OrderResult result={result} />}
    </div>
  )
}

export default App