import { ReactNode } from 'react'
import BlankLayout from 'src/@core/layouts/BlankLayout'

const Home = () => {
  return (
    <>
      <p>Yapım Aşamasında</p>
    </>
  )
}

Home.getLayout = (page: ReactNode) => <BlankLayout>{page}</BlankLayout>
export default Home
