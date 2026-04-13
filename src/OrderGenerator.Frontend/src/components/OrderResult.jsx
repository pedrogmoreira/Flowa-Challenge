function OrderResult({ result }) {
  return (
    <div className="result">
      <h2>Execution Result</h2>
      <table>
        <tbody>
          <tr><td>ClOrdID</td><td>{result.clOrdID}</td></tr>
          <tr><td>Symbol</td><td>{result.symbol}</td></tr>
          <tr><td>Side</td><td>{result.side}</td></tr>
          <tr><td>Executed Qty</td><td>{result.executedQty}</td></tr>
          <tr><td>Avg Price</td><td>{result.avgPrice}</td></tr>
          <tr><td>Status</td><td>{result.status}</td></tr>
        </tbody>
      </table>
    </div>
  )
}

export default OrderResult