import { NextApiRequest, NextApiResponse } from 'next'


export default async function handle (req: NextApiRequest, res: NextApiResponse) {
  const apiUrl = process.env.NEXT_PUBLIC_API_URL

  if (apiUrl === undefined) {
    throw new Error('NEXT_PUBLIC_API_URL is undefined')
  }

  try {
    const accessToken = req.cookies.access_token

    const headers: Record<string, string> = {}

    if (accessToken) {
      headers['Authorization'] = `Bearer ${accessToken}`
    }

    const response = await fetch(`${apiUrl}${req.url}`, {
      method: req.method,
      headers: headers
    })

    const body = await response.arrayBuffer()
    res.writeHead(response.status, response.statusText).end(Buffer.from(body))
  }
  catch(err) {
    if (err instanceof Response) {
      const body = await err.arrayBuffer()
      res.writeHead(err.status, err.statusText).end(Buffer.from(body))
    }
  }
}
