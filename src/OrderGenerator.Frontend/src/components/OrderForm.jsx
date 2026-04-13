import { useState } from 'react'
import axios from 'axios'

const SYMBOLS = ['PETR4', 'VALE3', 'VIIA4']
const SIDES = ['Buy', 'Sell']

function OrderForm({ setResult, setError, setLoading, loading }) {
  const [form, setForm] = useState({
    symbol: 'PETR4',
    side: 'Buy',
    quantity: '',
    price: ''
  })

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value })
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setResult(null)
    setError(null)
    setLoading(true)

    try {
      const response = await axios.post('/api/orders', {
        symbol: form.symbol,
        side: form.side,
        quantity: parseInt(form.quantity),
        price: parseFloat(form.price)
      })
      setResult(response.data)
    } catch (err) {
      setError(err.response?.data?.title ?? 'An error occurred. Please try again.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <form onSubmit={handleSubmit}>
      <div className="field">
        <label>Symbol</label>
        <select name="symbol" value={form.symbol} onChange={handleChange}>
          {SYMBOLS.map(s => <option key={s}>{s}</option>)}
        </select>
      </div>

      <div className="field">
        <label>Side</label>
        <select name="side" value={form.side} onChange={handleChange}>
          {SIDES.map(s => <option key={s}>{s}</option>)}
        </select>
      </div>

      <div className="field">
        <label>Quantity</label>
        <input
          type="number"
          name="quantity"
          value={form.quantity}
          onChange={handleChange}
          min="1"
          max="99999"
          required
        />
      </div>

      <div className="field">
        <label>Price</label>
        <input
          type="number"
          name="price"
          value={form.price}
          onChange={handleChange}
          min="0.01"
          max="999.99"
          step="0.01"
          required
        />
      </div>

      <button type="submit" disabled={loading}>
        {loading ? 'Sending...' : 'Send Order'}
      </button>
    </form>
  )
}

export default OrderForm