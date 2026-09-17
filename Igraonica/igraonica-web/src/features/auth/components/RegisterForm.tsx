import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useRegister } from '../hooks/useAuthActions'
import { Input } from '../../../components/ui/Input'
import { Button } from '../../../components/ui/Button'
import { Card } from '../../../components/ui/Card'

export function RegisterForm() {
  const [form, setForm] = useState({ email: '', password: '', firstName: '', lastName: '', phoneNumber: '' })
  const register = useRegister()
  const navigate = useNavigate()

  const update = (key: keyof typeof form) => (e: React.ChangeEvent<HTMLInputElement>) =>
    setForm((f) => ({ ...f, [key]: e.target.value }))

  return (
    <Card className="mx-auto max-w-md">
      <h2 className="mb-4 text-xl font-bold text-ink">Registracija</h2>
      <form
        className="flex flex-col gap-4"
        onSubmit={(e) => {
          e.preventDefault()
          register.mutate(form, { onSuccess: () => navigate('/prijava') })
        }}
      >
        <div className="grid grid-cols-2 gap-3">
          <Input label="Ime" value={form.firstName} onChange={update('firstName')} required />
          <Input label="Prezime" value={form.lastName} onChange={update('lastName')} required />
        </div>
        <Input label="Adresa elektronske poste" type="email" value={form.email} onChange={update('email')} required />
        <Input label="Broj telefona" value={form.phoneNumber} onChange={update('phoneNumber')} required />
        <Input
          label="Lozinka"
          type="password"
          value={form.password}
          onChange={update('password')}
          minLength={8}
          required
        />
        <Button type="submit" isLoading={register.isPending}>
          Registruj se
        </Button>
      </form>
    </Card>
  )
}
