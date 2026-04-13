describe('Order Generator', () => {
  beforeEach(() => {
    cy.visit('http://localhost:5173')
  })

  it('should display the order form', () => {
    cy.contains('Order Generator')
    cy.get('select[name="symbol"]').should('exist')
    cy.get('select[name="side"]').should('exist')
    cy.get('input[name="quantity"]').should('exist')
    cy.get('input[name="price"]').should('exist')
    cy.get('button[type="submit"]').should('exist')
  })

  it('should submit a valid order and display execution result', () => {
    cy.get('select[name="symbol"]').select('PETR4')
    cy.get('select[name="side"]').select('Buy')
    cy.get('input[name="quantity"]').clear().type('100')
    cy.get('input[name="price"]').clear().type('10.50')
    cy.get('button[type="submit"]').click()

    cy.contains('Execution Result', { timeout: 15000 })
    cy.contains('Filled')
    cy.contains('PETR4')
    cy.contains('Buy')
  })

  it('should block submission with empty quantity', () => {
    cy.get('input[name="quantity"]').clear()
    cy.get('button[type="submit"]').click()
    cy.get('input[name="quantity"]:invalid').should('exist')
  })

  it('should block submission with empty price', () => {
    cy.get('input[name="price"]').clear()
    cy.get('button[type="submit"]').click()
    cy.get('input[name="price"]:invalid').should('exist')
  })
})